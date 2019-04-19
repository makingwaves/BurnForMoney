
import authFetch from "../../../components/Authentication/AuthFetch";
const api_url = process.env.REACT_APP_DASHBOARD_API_URL;


// Categories
export function getCategories() {

    return authFetch(api_url + "api/activities/categories")
        .then(res => res.json())
}


// Athletes

export function getAthletes() {

    return authFetch(api_url + "api/athletes")
        .then(res => res.json())

}

export function getAthleteActivities(athleteId) {
   return authFetch(api_url + "api/athletes/" +  athleteId + "/activities")
        .then(res => res.json())
      
}

export function addNewActivity(userProfile, newActivity) {
    return authFetch(`${api_url}api/athletes/${userProfile}/activities`, 'POST', JSON.stringify(newActivity))

}

export function authorizeWithStrava(userProfile, code) {
    return authFetch(`${process.env.REACT_APP_DASHBOARD_API_URL}api/athletes/${userProfile}/strava_code`, "POST", JSON.stringify(code))
   
}

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




