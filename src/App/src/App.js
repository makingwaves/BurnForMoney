import React, {Component} from 'react';
import {
  BrowserRouter as Router,
  Route,
  Switch
} from 'react-router-dom';

import Home from './screens/home/Home';
import StravaAuth from './screens/stravaAuth/StravaAuth';
import StravaConfirmation from './screens/stravaAuth/StravaConfirmation';
import AppDashboard from './screens/dashboard/AppDashboard';
import AppTvboard from './screens/tvboard/AppTvboard';

import {authComponent, SignInHandler, SilentCallbackHandler} from "./components/Authentication/AuthComponent";
import SimpleLoginPage from "./components/Authentication/SimpleLoginPage";

function  App(){
  let authDashboard = authComponent(AppDashboard, SimpleLoginPage);

  return (
    <Router>
      <div className="App">
        <Switch>
          <Route exact path="/" component={Home} />
          <Route path="/strava" component={StravaAuth} />
          <Route path="/strava-confirmation" component={StravaConfirmation} />
          <Route path="/dashboard" component={authDashboard} />
          <Route path="/tvboard" component={AppTvboard} />
          <Route path="/auth/signin" component={SignInHandler} />
          <Route path="/auth/silent" component={SilentCallbackHandler} />
          
          {/* <Route path="/auth/login" component={LoginPage} />
          <Route path="/auth/signin" component={SignInHandler} /> */}
        </Switch>
      </div>
    </Router>
  );
}

export default App;
