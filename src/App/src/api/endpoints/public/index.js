import { api } from './api';

export function getTotalNumbers() {

    return api.get('api/totalnumbers')
        .then(res => res);       
}
