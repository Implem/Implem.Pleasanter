// ポート 59803 を掴んだままのアプリを終了する。
//
// dotnet run はホスト (dotnet) とアプリ本体 (Implem.Pleasanter) の 2 プロセス構成で、
// タスクを止めてもアプリ本体が孤児として残ることがある。
// アプリ本体を落とせばホストも連動して終了する。
//
// dotnet CLI に停止コマンドは無く、OS にもポート指定で kill する仕組みは無いため、
// ポートから PID を引いて落とす。起動時に PID を控えておく必要は無い。
//
// npm run dev (:3000) は対象外。ウォッチャーはタスクとして止める。

import { execFileSync } from 'node:child_process';

const port = 59803;
const isWindows = process.platform === 'win32';

// 出力が 1MB を超えると既定では ENOBUFS になるため広げておく
const execOptions = { encoding: 'utf8', maxBuffer: 10 * 1024 * 1024 };

function abort(message) {
    console.error(message);
    process.exit(1);
}

function listeningPids() {
    try {
        if (isWindows) {
            // netstat の状態表記は日本語環境でも LISTENING のまま
            const output = execFileSync('netstat', ['-ano', '-p', 'tcp'], execOptions);
            const pids = new Set();
            for (const line of output.split(/\r?\n/)) {
                const columns = line.trim().split(/\s+/);
                if (columns.length < 5 || columns[3] !== 'LISTENING') {
                    continue;
                }
                if (columns[1].endsWith(`:${port}`)) {
                    pids.add(Number(columns[4]));
                }
            }
            return [...pids];
        }
        const args = ['-nP', `-iTCP:${port}`, '-sTCP:LISTEN', '-t'];
        const output = execFileSync('lsof', args, execOptions);
        return [...new Set(output.split(/\s+/).filter(Boolean).map(Number))];
    } catch (error) {
        if (error.code === 'ENOENT') {
            abort(`${isWindows ? 'netstat' : 'lsof'} が見つかりません。手動で停止してください`);
        }
        // lsof は該当が無いと終了コード 1 を返す。これだけが正常系。
        if (!isWindows && error.status === 1) {
            return [];
        }
        // それ以外を空扱いにすると、生きているのに free と誤報してしまう
        abort(`ポートの確認に失敗しました: ${error.message}`);
    }
}

const pids = listeningPids();
if (pids.length === 0) {
    console.log(`[${port}] free`);
}

// 孤児の後始末が目的なので SIGKILL で確実に落とす。
// Windows では signal は無視され常に強制終了になる。
for (const pid of pids) {
    try {
        process.kill(pid, 'SIGKILL');
        console.log(`[${port}] stopped PID=${pid}`);
    } catch (error) {
        if (error.code === 'ESRCH') {
            console.log(`[${port}] PID=${pid} は既に終了している`);
            continue;
        }
        // 権限不足などで落とせなかった場合。成功として扱うとポートが残る
        console.error(`[${port}] PID=${pid} を終了できません: ${error.code ?? error.message}`);
        process.exitCode = 1;
    }
}
