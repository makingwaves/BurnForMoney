import {IDENTITY_CONFIG} from "../utlis/authConst"
import { UserManager, WebStorageStateStore, Log } from "oidc-client";


export default class AuthService {

    UserManager;
    accessToken;

    constructor() {
        this.UserManager = new UserManager({
            ...IDENTITY_CONFIG,
            userStore: new WebStorageStateStore({store: window.localStorage})
        });

       this.UserManager.events.addUserLoaded(user => {
           this.accessToken = user.access_token;
           localStorage.setItem("access_token", user.access_token);
           localStorage.setItem("id_token", user.id_token);
           
           if (window.location.href.indexOf("signin-oidc") !== -1) {
               this.navigateToScreen();
           }

       });

        this.UserManager.events.addSilentRenewError(e => {
            console.log("silent renew error", e.message);
        });

        this.UserManager.events.addAccessTokenExpired(() => {
            console.log("token expired");
            this.signinSilent();
        });

    }

    signinRedirectCallback = () => {
        this.UserManager.signinRedirectCallback().then(() => {
            console.log('signinRedirectCallback');
            
        });
    };

    getUser = async () => {
        const user = await this.UserManager.getUser();
        if (!user) {
            return await this.UserManager.signinRedirectCallback();
        }
        return user;
    };

   
    signinRedirect = () => {
        localStorage.setItem("redirectUri", window.location.pathname);
        this.UserManager.signinRedirect({});
    };

    navigateToScreen = () => {
        const redirectUri = !!localStorage.getItem("redirectUri")
            ? localStorage.getItem("redirectUri")
            : "/dashboard";


        console.log('redirectUri', redirectUri);
        window.location.replace(redirectUri)   
     
    };

    isAuthenticated = () => {
        const access_token = localStorage.getItem("access_token");
        return !!access_token;
    };


    signinSilent = () => {
        this.UserManager.signinSilent()
            .then(user => {
                console.log("signed in", user);
            })
            .catch(err => {
                console.log(err);
            });
    };

    signinSilentCallback = () => {
        this.UserManager.signinSilentCallback();
    };

   
    // TO CHANGE
    static isAuthenticated = () => {
        const access_token = localStorage.getItem("access_token");
        return !!access_token;
    };

    // TO CHANGE
    static getAuthToken = () => {
        const access_token = localStorage.getItem("access_token");
        return access_token;
    }

}