import type { DisplayId } from '@pleasanter/scripts/generals/display';
const typeKeys = ['Class', 'Num', 'Date', 'Description', 'Check', 'Attachments'] as const;

const pDisplay = (key: string, replaceValue: string | number = '') => {
    let text: string;
    const displayKey = key as DisplayId;
    const type = typeKeys.find(type => key.includes(type));
    if ($p.display(displayKey) !== key) {
        text = $p.display(displayKey);
    } else if (key.includes('_Links-')) {
        const suffix = key.split('_Links-')[1];
        text = $p.display('Links') + suffix;
    } else if (type !== undefined) {
        const suffix = key.split(type)[1];
        text = $p.display(type) + suffix;
    } else {
        text = key;
    }

    const doc = new DOMParser().parseFromString(text, 'text/html');
    text = doc.body.textContent ?? text;
    if (replaceValue && text) {
        text = text.replace(/\{0\}/g, String(replaceValue));
    }
    return text;
};

export { pDisplay };
