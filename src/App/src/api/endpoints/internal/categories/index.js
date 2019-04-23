
import authFetch from "../../../../components/Authentication/AuthFetch";
const api_url = process.env.REACT_APP_DASHBOARD_API_URL;


// Categories
export function getCategories() {

    return authFetch(api_url + "api/activities/categories")
        .then(res => res.json())
}





