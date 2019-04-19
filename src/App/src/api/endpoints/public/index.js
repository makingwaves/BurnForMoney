import authFetch from "../../../components/Authentication/AuthFetch";

const api_url = process.env.REACT_APP_API_URL;

export function getTotalNumbersAuth() {
    return authFetch(`${api_url}/api/totalnumbers`)
        .then(res => res.json())
      
}

export function getTotalNumbers() {
    return fetch(`${api_url}/api/totalnumbers`)
        .then(res => res.json())

}