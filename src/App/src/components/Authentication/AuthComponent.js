import React, {Component} from 'react';
import { AuthManager } from "./AuthManager"

class SilentCallbackHandler extends Component {
    componentDidMount(){
        let mgr = new AuthManager();
        mgr.signinSilentCallback()
        .then(_ => {
            console.log("Token refreshed");
        })
        .catch(err => {
            console.log(err);
        });
    }

    render () {
        return null;
    }
}

class SignInHandler extends Component {

    getParameterByName(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, '\\$&');
        var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    updateBrowserLocation(){
        let returnUrl = this.getParameterByName("redirect");

        window.history.replaceState({},
            window.document.title,
            window.location.origin);

        if(returnUrl)
            window.location = returnUrl;
        else
            window.location = "/";
    }

    componentDidMount(){
        let mgr = new AuthManager();
        mgr.signinRedirectCallback().then(() => {
            this.updateBrowserLocation();
            }, error => {
                console.error(error);
            });
    }

    render () {
        return null;
    }
}

function authComponent(WrappedComponent, LoginComponent) {
    return class extends  Component{
        constructor(props){
            super(props);
            this._authManager = new AuthManager();
            this.state = {
                state: null
            }
        }

        componentDidMount() {
            this._authManager.getUser()
            .then(user => {
                this.setState({state: user == null ?
                    "unauthenticated" :
                    "authenticated"});
            })
            .catch(e => {
                console.log(e);
            });
        }

        render(){
            switch(this.state.state) {
                case null:
                    return null;
                case "unauthenticated":
                    return <LoginComponent {...this.props} />;
                case "authenticated":
                    return <WrappedComponent {...this.props} />;
            }
        }
    };
}

export {SignInHandler, SilentCallbackHandler, authComponent};
