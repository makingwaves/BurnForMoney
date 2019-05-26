import { IDENTITY_CONFIG, METADATA_OIDC } from "../utlis/authConst"
import { UserManager, WebStorageStateStore, Log } from "oidc-client";

class OidcAuthProvider {

    userManager;
    accessToken;

    constructor() {
        this.userManager = new UserManager({
            ...IDENTITY_CONFIG,
            userStore: new WebStorageStateStore({ store: window.localStorage }),
            
        });

        this.userManager.events.addUserLoaded(user => {
            this.accessToken = user.access_token;
            localStorage.setItem("access_token", user.access_token);
            localStorage.setItem("id_token", user.id_token);

            if (window.location.href.indexOf("signin-oidc") !== -1) {
                this.navigateToScreen();
            }

        });

        this.userManager.events.addSilentRenewError(e => {
            console.log("silent renew error", e.message);
        });

        this.userManager.events.addAccessTokenExpired(() => {
            console.log("token expired");
            this.signinSilent();
        });

    }

    isAuthenticated = () => {
        const access_token = localStorage.getItem("access_token");
        return !!access_token;
    };

    getAuthToken = () => {
        const access_token = localStorage.getItem("access_token");
        return access_token;
    }

    getUser = () => {
        return this.userManager.getUser();
    }

    signinRedirectCallback = () => {
        this.userManager.signinRedirectCallback().then(() => {
            console.log('signinRedirectCallback');

        });
    };

    signinRedirect = () => {
        localStorage.setItem("redirectUri", window.location.pathname);
        this.userManager.signinRedirect({});
    };

    navigateToScreen = () => {
        const redirectUri = !!localStorage.getItem("redirectUri")
            ? localStorage.getItem("redirectUri")
            : "/dashboard";

        window.location.replace(redirectUri)

    };

    signinSilent = () => {
        this.userManager.signinSilent()
            .then(user => {
                console.log("signed in", user);
            })
            .catch(err => {
                console.log(err);
            });
    };

    signinSilentCallback = () => {
        this.userManager.signinSilentCallback();
    };

   signoutRedirect = () => {
        this.userManager.signoutRedirect();
    }


}

export const oidcAuthProvider = new OidcAuthProvider();







