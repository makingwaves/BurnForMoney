import { api } from '../../internal/api';


// Categories
export function getCategories() {

    return api.get('api/activities/categories')
        .then(res => res);

}





