import axios from 'axios'
import auth from '../../../services/authService';

function getApiOptions() {
    const apiUrl = process.env.REACT_APP_API_URL;

    const options = {
        baseURL: apiUrl,
        headers: {
            Accept: 'application/json',
            'Content-Type': 'application/json',          
        },
    };

    if (auth.isAuthenticated()) {
        options.headers['Authorization'] = `Bearer ${auth.getAuthToken()}`
       
  
    }

    return options;
}

export const api = axios.create(getApiOptions());

api.interceptors.response.use(
    response => response.data,
    error => {
        if (error.response && error.response.status === 401 && auth.isAuthenticated()) {
            console.log(error);
        } else if (error.response && error.response.status >= 500) {
            console.log(error);
        }
        return Promise.reject(error);
    },
);