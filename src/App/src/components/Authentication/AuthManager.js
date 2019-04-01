import { UserManager, UserManagerSettings, User } from 'oidc-client';

export class AuthManager {
    constructor(){
        var settings = {
            authority: "https://dd-bfm-backend.azurewebsites.net/",
            client_id: "test1",
            redirect_uri: `http://localhost:3000/auth/signin?redirect=${window.location.href}`,
            silent_redirect_uri: "http://localhost:3000/auth/silent",
            post_logout_redirect_uri: `http://localhost:3000`,
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

};