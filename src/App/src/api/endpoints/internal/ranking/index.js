
import authFetch from "../../../../components/Authentication/AuthFetch";
const api_url = process.env.REACT_APP_DASHBOARD_API_URL;


// Ranking

export function getRanking() {

    return authFetch(api_url + "api/ranking")
        .then(res => res.json())

}

export function getRankingByDate(month, year) {
   return authFetch(`${api_url}/api/ranking?month=${month}&year=${year}`)
        .then(res => res.json())
        
}

export function getCategoryRanking(category) {

    return authFetch(this.api_url + "api/ranking/" + category)
        .then(res => res.json())

}




