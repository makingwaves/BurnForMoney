import { api } from '../../internal/api';


// Athletes

export function getAthletes() {

    return api.get('api/athletes')
        .then(res => res);
}

export function getAthleteActivities(athleteId) {

    return api.get('api/athletes/' + athleteId + '/activities')
        .then(res => res);
        
}

export function addNewActivity(userProfile, newActivity) {

    return api.post(`api/athletes/${userProfile}/activities`, JSON.stringify(newActivity));
}

export function authorizeWithStrava(userProfile, code) {

    return api.post(`api/athletes/${userProfile}/strava_code`, JSON.stringify(code)); 
}




