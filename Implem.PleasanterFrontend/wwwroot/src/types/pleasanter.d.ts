import { type Display } from '../scripts/generals/display';
import { type Passkey } from '../scripts/generals/passkey';

declare global {
    interface Window {
        $p: {
            isForm: () => boolean;
            multiUpload: (
                url: string,
                data: FormData,
                $control?: JQuery<HTMLElement>,
                statusBar?: object,
                callback?: function
            ) => void;
            ajax: (
                url: string,
                methodType: string,
                data?: unknown,
                $control?: JQuery<HTMLElement> | null,
                _async?: boolean,
                clearMessage?: boolean
            ) => number | false | void;
            getData: (target: JQuery<HTMLElement>) => Record<string, unknown>;
            openVideo: (controlId: string) => void;
            openEditorColumnDialog: (target: JQuery<HTMLElement>) => void;
            set: (target: JQuery<HTMLElement>, value: string) => void;
            send: (target: JQuery<HTMLElement>) => void;
            syncSend: (target: JQuery<HTMLElement>, value?: string) => void;
            resetEditorColumn: (target: JQuery<HTMLElement>) => void;
            loading: (target?: JQuery<HTMLBUTTONElement>) => void;
            loaded: () => void;
            enableColumns: (
                event: Event,
                $control: JQuery<HTMLElement>,
                columnHeader: string,
                columnsTypeControl: string
            ) => void;
            moveColumns: (
                event: Event,
                $control: JQuery<HTMLElement>,
                columnHeader: string,
                isKeepSource?: boolean,
                isJoin?: boolean,
                type?: string
            ) => void;
            modal?: Record<string, HTMLElement>;
        } & typeof Passkey &
            typeof Display;
    }

    const $p: Window['$p'];
}

export {};
