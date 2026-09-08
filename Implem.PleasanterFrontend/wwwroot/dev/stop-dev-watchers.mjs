// npm run dev のプロセスツリーを丸ごと終了する。
//
// dev は concurrently で 5 つのウォッチャー
// (vite x2 / sass / テーマ配布 / browser-sync) を束ねている。
// browser-sync (:3000) だけを落としても残り 4 つが動き続けるため、
// ポート単位ではなくプロセスツリー単位で終了する。
//
// 起点はこのリポジトリのパスを参照しているプロセスに限定し、
// そこから根まで遡ることで、browser-sync が既に死んでいるツリーも拾える。
//
// アプリ (:59803) は対象外。そちらは dev/stop-pleasanter.mjs が担当する。

import { execFileSync } from 'node:child_process';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const wwwRoot = resolve(dirname(fileURLToPath(import.meta.url)), '..');

// パス比較用の正規化。Windows ではドライブレターの大小と区切り文字が
// 揃わないことがあるため（VS Code は c:、npm は C: を出す）両方を吸収する。
function normalize(value) {
    return value.replace(/\\/g, '/').toLowerCase();
}

const wwwRootKey = normalize(wwwRoot);
const isWindows = process.platform === 'win32';
const execOptions = { encoding: 'utf8', maxBuffer: 32 * 1024 * 1024 };

// ツリーを遡る際の継続条件。npm run dev の内部は cmd と node が交互に入れ子になり、
// 中間のラッパーも辿れないと途中で止まって葉を根と誤認する。
// 起点は常にこのリポジトリ配下なので、一致するのはその祖先に限られる。
// 判定は normalize 済みの文字列に対して行うため、目印も小文字・スラッシュで書く。
const treeMarkers = [wwwRootKey, 'concurrently', 'npm-cli.js', 'npm run', 'run dev', 'watch:', 'vite build', 'dev/'];

const runtimeNames = ['node.exe', 'cmd.exe', 'node', 'sh', 'bash'];

function abort(message) {
    console.error(message);
    process.exit(1);
}

function run(command, args) {
    try {
        return execFileSync(command, args, execOptions);
    } catch (error) {
        if (error.code === 'ENOENT') {
            abort(`${command} が見つかりません。手動で停止してください`);
        }
        throw error;
    }
}

// pid -> { ppid, name, command }
// name は実行ファイル名。コマンドライン文字列で node/cmd を判定すると、
// 引数にたまたまその語を含む無関係なプロセス（シェル等）を巻き込む。
function processTable() {
    const table = new Map();
    if (isWindows) {
        const script =
            'Get-CimInstance Win32_Process | Select-Object ProcessId,ParentProcessId,Name,CommandLine | ConvertTo-Json -Compress';
        const output = run('powershell', ['-NoProfile', '-Command', script]);
        for (const row of JSON.parse(output)) {
            table.set(row.ProcessId, {
                ppid: row.ParentProcessId,
                name: (row.Name ?? '').toLowerCase(),
                command: normalize(row.CommandLine ?? '')
            });
        }
        return table;
    }
    const output = run('ps', ['-eo', 'pid=,ppid=,comm=,args=']);
    for (const line of output.split('\n')) {
        const matched = line.trim().match(/^(\d+)\s+(\d+)\s+(\S+)\s+(.*)$/);
        if (matched) {
            table.set(Number(matched[1]), {
                ppid: Number(matched[2]),
                name: matched[3].split('/').pop().toLowerCase(),
                command: normalize(matched[4])
            });
        }
    }
    return table;
}

function isRuntime(entry) {
    return entry ? runtimeNames.includes(entry.name) : false;
}

function isTreeMember(entry) {
    return isRuntime(entry) && treeMarkers.some(marker => entry.command.includes(marker));
}

// 根が dev のツリーかどうか。
// 起点はコマンドラインにこのリポジトリのパスを含むだけで成立するため、
// ウォッチャーと無関係なプロセスも起点になりうる
// (例: cmd /c cd /d <wwwroot> && node ...)。
// その子孫を無条件に落とすと無関係なツリーを巻き込むので、
// パス以外の目印を持つことを根の側で確かめる。
//
// 根は npm run dev を起動したシェル ("cmd.exe" /c npm run dev) か、
// その下の npm ラッパーのどちらかになり、前者は "npm run" / "run dev"、
// 後者は "npm-cli.js" に当たる。npm スクリプト経由で起動する限りは
// いずれかを必ず含むが、起動方法を変えてこれらの語がコマンドラインから
// 消えると、根が除外されてウォッチャーが停止されない
// （「動作していません」と表示されるだけで残り続ける）。
const devMarkers = treeMarkers.filter(marker => marker !== wwwRootKey);

function isDevRoot(entry) {
    return entry ? devMarkers.some(marker => entry.command.includes(marker)) : false;
}

function findRoot(pid, table) {
    let current = pid;
    // 親を辿れなくなるまで遡る。上限は PID 再利用による循環への保険
    for (let depth = 0; depth < 20; depth++) {
        const parentId = table.get(current)?.ppid;
        if (parentId === undefined || !isTreeMember(table.get(parentId))) {
            break;
        }
        current = parentId;
    }
    return current;
}

function treeMembers(rootId, table) {
    const childrenOf = new Map();
    for (const [pid, entry] of table) {
        if (!childrenOf.has(entry.ppid)) {
            childrenOf.set(entry.ppid, []);
        }
        childrenOf.get(entry.ppid).push(pid);
    }
    const members = [];
    const pending = [rootId];
    while (pending.length > 0) {
        const current = pending.pop();
        if (members.includes(current)) {
            continue;
        }
        members.push(current);
        pending.push(...(childrenOf.get(current) ?? []));
    }
    return members;
}

const table = processTable();
const seeds = [...table.entries()]
    .filter(([, entry]) => isRuntime(entry) && entry.command.includes(wwwRootKey))
    .map(([pid]) => pid);

const roots = [...new Set(seeds.map(pid => findRoot(pid, table)))]
    .filter(rootId => isDevRoot(table.get(rootId)));
if (roots.length === 0) {
    console.log('npm run dev : 動作していません');
}

for (const rootId of roots) {
    // PID は取得済みのスナップショットから拾っているため、
    // 途中で孤児になっても取りこぼさない。順序は問わない。
    const members = treeMembers(rootId, table);
    for (const pid of members) {
        try {
            process.kill(pid, 'SIGKILL');
        } catch (error) {
            if (error.code !== 'ESRCH') {
                console.error(`PID=${pid} を終了できません: ${error.code ?? error.message}`);
                process.exitCode = 1;
            }
        }
    }
    console.log(`npm run dev : ツリーを終了しました (root PID=${rootId}, ${members.length} プロセス)`);
}
