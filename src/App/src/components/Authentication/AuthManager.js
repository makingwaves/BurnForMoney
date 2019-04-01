import { UserManager, UserManagerSettings, User } from 'oidc-client';

export class AuthManager {
    constructor(){
        var settings = {
            authority: process.env.REACT_APP_OIDC_AUTH_URL,
            client_id: process.env.REACT_APP_OIDC_CLIENT_ID,
            redirect_uri: `${window.location.origin}/auth/signin?redirect=${window.location.href}`,
            silent_redirect_uri: `${window.location.origin}/auth/silent`,
            post_logout_redirect_uri: window.location.origin,
            response_type: 'id_token token',
            scope: 'openid'
        };

        this._userManager = new UserManager(settings);
    }
    
    getUser(){
        return this._userManager.getUser();
    }
    
    login(){
        return this._userManager.signinRedirect();
    }
    
    renewToken(){
        return this._userManager.signinSilent();
    }
    
    logout(){
        return this._userManager.signoutRedirect();
    }

    signinRedirectCallback(){
        return this._userManager.signinRedirectCallback();
    }

    signinSilentCallback (){
        return this._userManager.signinSilentCallback();
    }
};