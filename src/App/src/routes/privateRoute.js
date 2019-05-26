import React from "react";
import { Route } from "react-router-dom";
import { AuthService } from "../services/authService"


export const PrivateRoute = ({ component, ...rest }) => {
    const renderFn = (Component) => (props) => {
        if (!!Component && AuthService.isAuthenticated()) {
            return <Component {...props} />;
        } else {
            AuthService.login();
            return <span>loading private route</span>;
        }
    };

    return <Route {...rest} render={renderFn(component)} />;
};