import axios, { AxiosError } from 'axios';

const apiCall = (option: {
    url: string;
    method: string;
    data?: FormData | object;
    params?: object;
    headers?: object;
    alert?: (message: string, note?: string) => void;
    onSuccess?: (data: unknown) => void;
    onError?: (data?: unknown) => void;
}) => {
    axios({
        method: option.method,
        url: option.url,
        data: option.data,
        params: option.params,
        headers: option.headers
    })
        .then(response => {
            option.onSuccess?.(response.data);
        })
        .catch(error => {
            const err = error as AxiosError;
            if (err.response && err.response.data) {
                option.onError?.(err.response.data);
            } else if (option.onError) {
                option.onError();
            } else if (option.alert) {
                option.alert('An error occurred while connecting to the API. Please try again.');
            }
        });
};

const promiseApi = (option: {
    url: string;
    method: string;
    data?: FormData | object;
    params?: object;
    headers?: object;
}) => {
    return axios({
        method: option.method,
        url: option.url,
        data: option.data,
        params: option.params,
        headers: option.headers
    });
};

export { apiCall, promiseApi };
