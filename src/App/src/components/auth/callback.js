import React from "react";
import { oidcAuthProvider } from '../../providers/oidcAuthProvider';


export const Callback = () =>  {
            oidcAuthProvider.signinRedirectCallback();
            return <span>loading callback</span>;
        };