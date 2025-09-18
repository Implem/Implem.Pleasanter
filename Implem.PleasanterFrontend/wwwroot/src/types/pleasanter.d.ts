declare global {
    interface Window {
        $p: {
            set: (target: JQuery<HTMLElement>, value: string) => void;

            modal?: Record<string, HTMLElement>;
        };
    }

    const $p: Window['$p'];
}

export {};
