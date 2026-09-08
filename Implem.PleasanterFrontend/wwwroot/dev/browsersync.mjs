import browserSync from 'browser-sync';
import { resolve, dirname } from 'path';
import { fileURLToPath } from 'url';

const __dirname = dirname(fileURLToPath(import.meta.url));
const rootDir = resolve(__dirname, '..');
const assetsDir = resolve(rootDir, '../../Implem.Pleasanter/wwwroot/assets');

const proxyTarget = process.env.PROXY_TARGET || 'http://localhost:59803';

const bs = browserSync.create();

bs.init({
    proxy: proxyTarget,
    // ソースマップの sources は出力先 (assets/css, assets/js) からの相対パスで
    // 4 階層遡るが、URL 上は 2 階層しか無く残りがルートで頭打ちになるため、
    // /Implem.PleasanterFrontend/wwwroot/... に解決される。
    // プロキシ先 (:59803) にその実体は無いので、ここで実ディレクトリを割り当てる。
    // これが無いと DevTools から .scss / .ts を開いたときに 404 になる。
    // node_modules も sources に現れる (normalize.css 等) ため併せて割り当てる。
    serveStatic: [
        {
            route: '/Implem.PleasanterFrontend/wwwroot/src',
            dir: resolve(rootDir, 'src')
        },
        {
            route: '/Implem.PleasanterFrontend/wwwroot/node_modules',
            dir: resolve(rootDir, 'node_modules')
        }
    ],
    files: [
        // css と themes で扱いが違うのは、本体が出力する href が違うため。
        //   assets/css/*.min.css   → "?v=<キャッシュバスト値>" 付き (HtmlStyles.LinkedStyles)
        //   assets/themes/**/*.css → クエリ無し                  (HtmlStyles.LinkStyles)
        // クエリ付きは既定の照合で注入対象を拾えないと見られるため、
        // css 側だけ *.css を明示してページ内の CSS を全件取り直させる。
        // 外す場合は、ブラウザを繋いだ状態で SCSS を保存し、
        // リロード無しで反映されることを確認すること。
        {
            match: [`${assetsDir}/css/**/*.css`],
            fn: function (_event, file) {
                if (file.endsWith('.css')) {
                    bs.reload('*.css');
                }
            }
        },
        // クエリが付かないため injectChanges による自動注入で足りる。
        `${assetsDir}/themes/**/*.css`,
        `${assetsDir}/js/**/*.js`
    ],
    open: false,
    notify: true,
    injectChanges: true,
    reloadDebounce: 300
});
