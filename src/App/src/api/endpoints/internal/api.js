import axios from 'axios'
import {AuthService} from '../../../services/authService'

function getApiOptions() {
    const apiUrl = process.env.REACT_APP_DASHBOARD_API_URL;

    const options = {
        baseURL: apiUrl,
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',          
        },
    };

    if (AuthService.isAuthenticated()) {
        options.headers['Authorization'] = `Bearer ${AuthService.getAuthToken()}`
       
  
    }

    return options;
}

export const api = axios.create(getApiOptions());

api.interceptors.response.use(
    response => response.data,
    error => {
        if (error.response && error.response.status === 401 && AuthService.isAuthenticated()) {
            AuthService.logout();
            console.log(error);
        } else if (error.response && error.response.status >= 500) {
            console.log(error);
        }
        return Promise.reject(error);
    },
);