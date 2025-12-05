declare global {
    interface Window {
        $p: {
            isForm: () => boolean;
            display: (defaultId: string) => string;
            multiUpload: (
                url: string,
                data: FormData,
                $control?: JQuery<HTMLElement>,
                statusBar?: object,
                callback?: function
            ) => void;
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
        };
    }

    const $p: Window['$p'];
}

export {};
