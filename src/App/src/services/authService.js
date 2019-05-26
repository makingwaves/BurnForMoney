import {oidcAuthProvider}   from '../providers/oidcAuthProvider';

export const AuthService = {
    login: oidcAuthProvider.signinRedirect.bind(this),
   // logout: oidcAuthProvider.signoutRedirect.bind(this),
    isAuthenticated: oidcAuthProvider.isAuthenticated.bind(this),
    getAuthToken: oidcAuthProvider.getAuthToken.bind(this),
    getUser: oidcAuthProvider.getUser.bind(this)
}

