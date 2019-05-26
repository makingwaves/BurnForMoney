import * as React from "react";
import { Route, Switch } from "react-router-dom";

import { Callback } from "../components/auth/callback";
import { PrivateRoute } from "./privateRoute";
import { SilentRenew } from "../components/auth/silentRenew";

import Home from '../apps/home/Home';
import BfmPanel from '../apps/bfmPanel/BfmPanel';
import AppTvboard from '../apps/tvboard/AppTvboard';


export const Routes = (
    <Switch>
        <Route exact={true} path="/signin-oidc" component={Callback} />     
        <Route exact={true} path="/silentrenew" component={SilentRenew} />
        <PrivateRoute path="/dashboard" component={BfmPanel} />
        <PrivateRoute path="/tvboard" component={AppTvboard} />
        <Route path="/" component={Home} />
    </Switch>
);