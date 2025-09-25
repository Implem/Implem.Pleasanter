declare global {
    interface Window {
        $p: {
            display: (defaultId: string) => string;
            multiUpload: (
                url: string,
                data: FormData,
                $control?: JQuery<HTMLElement>,
                statusBar?: object,
                callback?: function
            ) => void;
            set: (target: JQuery<HTMLElement>, value: string) => void;

            modal?: Record<string, HTMLElement>;
        };
    }

    const $p: Window['$p'];
}

export {};
