import { DisplayId } from './display';

type PasskeyDisplayId = Extract<DisplayId, `Passkey${string}`>;

const passkeyPermissionCheck = () =>
    'credentials' in navigator &&
    typeof navigator.credentials.create === 'function' &&
    typeof navigator.credentials.get === 'function';

const passkeyRegister = async (json: string) => {
    if (!passkeyPermissionCheck()) {
        alert(htmlDecodeDisplay('PasskeyNotAvailable'));
        return;
    }
    const interval = setInterval(() => {
        $p.loading();
    }, 200);

    try {
        const makeCredentialOptions = JSON.parse(json) as PublicKeyCredentialCreationOptions;

        makeCredentialOptions.challenge = coerceToArrayBuffer(makeCredentialOptions.challenge);
        makeCredentialOptions.user.id = coerceToArrayBuffer(makeCredentialOptions.user.id);

        if (makeCredentialOptions.excludeCredentials && makeCredentialOptions.excludeCredentials instanceof Array) {
            makeCredentialOptions.excludeCredentials = makeCredentialOptions.excludeCredentials.map(c => {
                c.id = coerceToArrayBuffer(c.id);
                return c;
            });
        }

        if (makeCredentialOptions.authenticatorSelection) {
            if (makeCredentialOptions.authenticatorSelection.authenticatorAttachment === null)
                makeCredentialOptions.authenticatorSelection.authenticatorAttachment = undefined;
        }

        const credential = await navigator.credentials.create({
            publicKey: makeCredentialOptions
        });

        if (!(credential instanceof PublicKeyCredential)) {
            throw new PasskeyError('PasskeyResponseInvalid');
        }

        const { response: attestationResponse } = credential;
        if (!(attestationResponse instanceof AuthenticatorAttestationResponse)) {
            throw new PasskeyError('PasskeyResponseInvalid');
        }

        // Move data into Arrays incase it is super long
        const attestationObject = new Uint8Array(attestationResponse.attestationObject);
        const clientDataJSON = new Uint8Array(attestationResponse.clientDataJSON);
        const rawId = new Uint8Array(credential.rawId);

        const data = {
            id: credential.id,
            rawId: coerceToBase64Url(rawId),
            type: credential.type,
            extensions: credential.getClientExtensionResults(),
            response: {
                attestationObject: coerceToBase64Url(attestationObject),
                clientDataJSON: coerceToBase64Url(clientDataJSON),
                transports: attestationResponse.getTransports()
            }
        };

        const action = 'MakeCredential';
        const url = $('#PasskeysForm').attr('action')!.replace('_action_', action.toLowerCase());
        $p.ajax(url, 'post', { data: JSON.stringify(data) });
    } catch (e) {
        catchError(e);
    } finally {
        clearInterval(interval);
        $p.loaded();
    }
};

const passkeyGetAssertionOptions = async () => {
    if (!passkeyPermissionCheck()) {
        alert(htmlDecodeDisplay('PasskeyNotAvailable'));
        return;
    }
    const url = `/passkeys/getassertionoptions`;
    $p.ajax(url, 'post', null);
};

const passkeyLogin = async (json: string) => {
    if (!$p.passkeyPermissionCheck()) {
        alert(htmlDecodeDisplay('PasskeyNotAvailable'));
        return;
    }
    const interval = setInterval(() => {
        $p.loading();
    }, 200);

    try {
        const makeAssertionOptions = JSON.parse(json) as PublicKeyCredentialRequestOptions;

        makeAssertionOptions.challenge = coerceToArrayBuffer(makeAssertionOptions.challenge);
        if (makeAssertionOptions.allowCredentials && makeAssertionOptions.allowCredentials instanceof Array) {
            makeAssertionOptions.allowCredentials.forEach(function (listItem) {
                listItem.id = coerceToArrayBuffer(listItem.id);
            });
        }

        // ask browser for credentials (browser will ask connected authenticators)
        const credential = await navigator.credentials.get({ publicKey: makeAssertionOptions });
        if (!(credential instanceof PublicKeyCredential)) {
            throw new PasskeyError('PasskeyResponseInvalid');
        }
        const { response: assertionResponse } = credential;
        if (!(assertionResponse instanceof AuthenticatorAssertionResponse)) {
            throw new PasskeyError('PasskeyResponseInvalid');
        }

        // Move data into Arrays incase it is super long
        const authData = new Uint8Array(assertionResponse.authenticatorData);
        const clientDataJSON = new Uint8Array(assertionResponse.clientDataJSON);
        const rawId = new Uint8Array(credential.rawId);
        const sig = new Uint8Array(assertionResponse.signature);
        const userHandle = assertionResponse.userHandle ? new Uint8Array(assertionResponse.userHandle) : null;
        const data = {
            id: credential.id,
            rawId: coerceToBase64Url(rawId),
            type: credential.type,
            extensions: credential.getClientExtensionResults(),
            response: {
                authenticatorData: coerceToBase64Url(authData),
                clientDataJSON: coerceToBase64Url(clientDataJSON),
                userHandle: userHandle !== null ? coerceToBase64Url(userHandle) : null,
                signature: coerceToBase64Url(sig)
            }
        };
        const Users_RememberMe = true;

        const queryString = window.location.search;
        const url = `/passkeys/makeassertion${queryString}`;
        const $control = $('#PasskeyLogin');
        $p.ajax(url, 'post', { data: JSON.stringify(data), Users_RememberMe }, $control);
    } catch (e) {
        catchError(e);
    } finally {
        clearInterval(interval);
        $p.loaded();
    }
};

const openPasskeyDialog = () => {
    if (!passkeyPermissionCheck()) {
        alert(htmlDecodeDisplay('PasskeyNotAvailable'));
        return;
    }

    const action = 'Passkeys';
    const url = $('#PasskeysForm').attr('action')!.replace('_action_', action.toLowerCase());
    $p.ajax(url, 'post', null, null, false);
    $('#PasskeyDialog').dialog({
        modal: true,
        width: 600,
        height: 'auto',
        resizable: false,
        closeOnEscape: true
    });
};

const openPasskeyChangeTitleDialog = async ($grid: JQuery<HTMLTableElement>) => {
    const passkeyId = $p.getData($grid)[$grid.attr('data-name')!] as string;
    const action = 'ChangeTitleDialog';
    const url = $('#PasskeysForm').attr('action')!.replace('_action_', action.toLowerCase());
    const data = { passkeyId };
    $p.ajax(url, 'post', data, null, false);
    $('#PasskeyChangeTitleDialog').dialog({
        modal: true,
        width: 400,
        height: 'auto',
        resizable: false,
        closeOnEscape: true
    });
};

const closePasskeyChangeTitleDialog = () => {
    $('#PasskeyChangeTitleDialog').dialog('close');
};

function coerceToArrayBuffer(thing: Uint8Array | ArrayBufferLike | number[] | string | BufferSource): ArrayBuffer {
    if (typeof thing === 'string') {
        // base64url to base64
        thing = thing.replace(/-/g, '+').replace(/_/g, '/');

        // base64 to Uint8Array
        const str = window.atob(thing);
        const bytes = new Uint8Array(str.length);
        for (let i = 0; i < str.length; i++) {
            bytes[i] = str.charCodeAt(i);
        }
        thing = bytes;
    }

    // Array to Uint8Array
    if (Array.isArray(thing)) {
        thing = new Uint8Array(thing);
    }

    // Uint8Array to ArrayBuffer
    if (thing instanceof Uint8Array) {
        thing = thing.buffer;
    }

    // error if none of the above worked
    if (!(thing instanceof ArrayBuffer)) {
        throw new PasskeyError('PasskeyResponseInvalid');
    }

    return thing;
}

function coerceToBase64Url(thing: ArrayBuffer | Uint8Array | number[] | string): string {
    // Array or ArrayBuffer to Uint8Array
    if (Array.isArray(thing)) {
        thing = Uint8Array.from(thing);
    }

    if (thing instanceof ArrayBuffer) {
        thing = new Uint8Array(thing);
    }

    // Uint8Array to base64
    if (thing instanceof Uint8Array) {
        let str = '';
        const len = thing.byteLength;

        for (let i = 0; i < len; i++) {
            str += String.fromCharCode(thing[i]);
        }
        thing = window.btoa(str);
    }

    if (typeof thing !== 'string') {
        throw new PasskeyError('PasskeyResponseInvalid');
    }

    // base64 to base64url
    // NOTE: "=" at the end of challenge is optional, strip it off here
    thing = thing.replace(/\+/g, '-').replace(/\//g, '_').replace(/=*$/g, '');

    return thing;
}

function htmlDecode(input: string): string {
    if (!input || !input.includes('&')) return input;
    const textarea = document.createElement('textarea');
    textarea.innerHTML = input;
    return textarea.value;
}

function htmlDecodeDisplay(input: PasskeyDisplayId): string {
    return htmlDecode($p.display(input));
}

class PasskeyError extends Error {
    constructor(message: PasskeyDisplayId) {
        super(htmlDecodeDisplay(message));
        this.name = 'PasskeyError';
    }
}

function catchError(e: unknown) {
    if (e instanceof Error) {
        if (e.name === 'NotAllowedError') {
            alert(htmlDecodeDisplay('PasskeyOperationTimeoutOrAbort'));
        } else if (e.name === 'PasskeyError') {
            alert(e.message);
        } else if (e.name === 'AbortError') {
            alert(htmlDecodeDisplay('PasskeyOperationAborted'));
        } else if (['SyntaxError', 'TypeError', 'DataError', 'InvalidStateError'].includes(e.name)) {
            alert(htmlDecodeDisplay('PasskeyResponseInvalid'));
        } else {
            alert(htmlDecodeDisplay('PasskeyOperationTimeoutOrAbort'));
        }
    } else {
        alert(htmlDecodeDisplay('PasskeyServerUnavailable'));
    }
}

export const Passkey = {
    passkeyPermissionCheck,
    passkeyRegister,
    passkeyGetAssertionOptions,
    passkeyLogin,
    openPasskeyDialog,
    openPasskeyChangeTitleDialog,
    closePasskeyChangeTitleDialog
};

Object.assign($p, Passkey);
