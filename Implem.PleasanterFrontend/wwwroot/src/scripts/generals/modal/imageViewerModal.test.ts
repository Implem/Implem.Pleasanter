// @vitest-environment happy-dom
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

import { installPleasanterGlobals } from '../../../test/setup';
import { ImageViewerModal } from './imageViewerModal';

/**
 * 画像ライトボックス (image-viewer-modal) の単体テスト。
 *
 * 実画像ロード (loadImageWithMinWait: img.onload / Date.now / setTimeout) はスタブし、
 * 「コレクションのカウンタ表示」「前後ナビの循環ロジック」「矢印キー操作」といった
 * 純粋なロジックだけを検証する。実際の画像読み込み・モーダル開閉は E2E の担当。
 */

/** マイクロタスク/タイマーを1周させて imgDisplay の .then を反映させる */
const flush = () => new Promise(resolve => setTimeout(resolve, 0));

function img(src: string) {
    const el = document.createElement('img');
    el.src = src;
    return el;
}

/** 非公開フィールド参照用 */
interface Privates {
    imgCurrent?: number;
    imgCollection?: string[];
}

/** 接続済みの modal を生成し、実画像ロードを「要求 src をそのまま返す」スタブへ差し替える */
function mountModal() {
    const modal = document.createElement('image-viewer-modal') as ImageViewerModal;
    document.body.appendChild(modal);
    // 実画像ロードはプロトタイプ側でスタブ (afterEach の restoreAllMocks で戻す)
    vi.spyOn(ImageViewerModal.prototype, 'loadImageWithMinWait').mockImplementation((src: string) =>
        Promise.resolve(img(src))
    );
    return modal;
}

const sd = (modal: ImageViewerModal) => modal.shadowRoot!;
const priv = (modal: ImageViewerModal) => modal as unknown as Privates;
const currentText = (modal: ImageViewerModal) => sd(modal).querySelector('.counter .current')!.textContent;
const viewerSrc = (modal: ImageViewerModal) => (sd(modal).querySelector('#imgViewer') as HTMLImageElement).src;
const asNodeList = (nodes: HTMLImageElement[]) => nodes as unknown as NodeListOf<HTMLImageElement>;

beforeEach(() => {
    // ui-modal が connectedCallback で window.$p.modal を参照するため必ず用意する
    installPleasanterGlobals();
    document.body.innerHTML = '';
});

afterEach(() => {
    vi.restoreAllMocks();
});

describe('show()', () => {
    it('複数画像: has-collection と 現在/最大 カウントを表示し、対象画像を出す', async () => {
        const modal = mountModal();
        const nodes = [img('https://h/a.png'), img('https://h/b.png'), img('https://h/c.png')];

        modal.show(nodes[1], asNodeList(nodes));

        expect(sd(modal).querySelector('ui-modal')!.hasAttribute('has-collection')).toBe(true);
        expect(currentText(modal)).toBe('2');
        expect(sd(modal).querySelector('.counter .max')!.textContent).toBe('3');
        expect(priv(modal).imgCurrent).toBe(1);

        await flush();
        expect(viewerSrc(modal)).toBe('https://h/b.png');
    });

    it('単一画像: コレクション表示せず、対象画像だけを出す', async () => {
        const modal = mountModal();
        const node = img('https://h/only.png');

        modal.show(node, asNodeList([node]));

        expect(sd(modal).querySelector('ui-modal')!.hasAttribute('has-collection')).toBe(false);
        await flush();
        expect(viewerSrc(modal)).toBe('https://h/only.png');
    });
});

describe('ナビゲーション (循環)', () => {
    function mountWithCollection(startIndex: number) {
        const modal = mountModal();
        const nodes = [img('https://h/a.png'), img('https://h/b.png'), img('https://h/c.png')];
        modal.show(nodes[startIndex], asNodeList(nodes));
        return modal;
    }

    it('次へ: 最後の画像から最初へ循環する', async () => {
        const modal = mountWithCollection(2); // 最後
        sd(modal).querySelector<HTMLButtonElement>('.counter .next')!.dispatchEvent(new MouseEvent('click'));

        expect(priv(modal).imgCurrent).toBe(0);
        await flush();
        expect(viewerSrc(modal)).toBe('https://h/a.png');
        expect(currentText(modal)).toBe('1');
    });

    it('前へ: 最初の画像から最後へ循環する', async () => {
        const modal = mountWithCollection(0); // 最初
        sd(modal).querySelector<HTMLButtonElement>('.counter .prev')!.dispatchEvent(new MouseEvent('click'));

        expect(priv(modal).imgCurrent).toBe(2);
        await flush();
        expect(viewerSrc(modal)).toBe('https://h/c.png');
        expect(currentText(modal)).toBe('3');
    });

    it('矢印キー: → で次、← で前へ移動しカレント数を更新する', async () => {
        const modal = mountWithCollection(0);

        sd(modal).dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowRight' }));
        expect(priv(modal).imgCurrent).toBe(1);
        await flush();
        expect(currentText(modal)).toBe('2');

        sd(modal).dispatchEvent(new KeyboardEvent('keydown', { key: 'ArrowLeft' }));
        expect(priv(modal).imgCurrent).toBe(0);
        await flush();
        expect(currentText(modal)).toBe('1');
    });
});

describe('画像ロード失敗時 (imgDisplay の catch)', () => {
    it('ロードが reject したら画像を表示せず、モーダルを閉じる (catch 経路)', async () => {
        const modal = document.createElement('image-viewer-modal') as ImageViewerModal;
        document.body.appendChild(modal);
        vi.spyOn(ImageViewerModal.prototype, 'loadImageWithMinWait').mockRejectedValue(new Error('load failed'));
        // catch の `customModalElem.open = false` (閉じる副作用) を捉えるため、実際に catch が
        // 触れる customModalElem 上の open setter を差し替える
        const uiModal = (modal as unknown as { customModalElem: HTMLElement }).customModalElem;
        const openSetter = vi.fn();
        Object.defineProperty(uiModal, 'open', { get: () => false, set: openSetter, configurable: true });

        // node は collection と同一インスタンスにする (別インスタンスだと show 内の indexOf が
        // -1 になり imgDisplay が呼ばれず catch を通らない)
        const a = img('https://h/a.png');
        const b = img('https://h/b.png');
        modal.show(a, asNodeList([a, b]));
        await flush();

        // then が走らないので画像 src は設定されない (catch でクローズ処理へ分岐)
        expect(sd(modal).querySelector('#imgViewer')!.getAttribute('src')).toBe('');
        expect(openSetter).toHaveBeenCalledWith(false); // catch でモーダルを閉じる
    });
});

describe('閉じたときのクリア (set onClosed)', () => {
    it('コレクションと画像をクリアし、渡したコールバックを呼ぶ', async () => {
        const modal = mountModal();
        modal.show(img('https://h/a.png'), asNodeList([img('https://h/a.png'), img('https://h/b.png')]));
        await flush();
        const uiModal = sd(modal).querySelector('ui-modal') as unknown as { onClosed?: () => void };
        const cb = vi.fn();
        modal.onClosed = cb;

        uiModal.onClosed!(); // モーダル閉鎖を模擬

        expect(cb).toHaveBeenCalledTimes(1);
        expect(sd(modal).querySelector('#imgViewer')!.getAttribute('src')).toBe('');
        expect(priv(modal).imgCollection).toBeUndefined();
        expect(sd(modal).querySelector('ui-modal')!.hasAttribute('has-collection')).toBe(false);
    });
});
