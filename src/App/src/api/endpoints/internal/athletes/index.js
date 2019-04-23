
import authFetch from "../../../../components/Authentication/AuthFetch";
const api_url = process.env.REACT_APP_DASHBOARD_API_URL;


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
    return authFetch(`${api_url}api/athletes/${userProfile}/strava_code`, "POST", JSON.stringify(code))
   
}




