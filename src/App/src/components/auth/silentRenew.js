import React from "react";

import { oidcAuthProvider } from '../../providers/oidcAuthProvider';

export const SilentRenew = () => {
            oidcAuthProvider.signinSilentCallback();
            return <span>loading renew</span>;
        };