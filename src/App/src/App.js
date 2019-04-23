import React from 'react';
import {
  BrowserRouter as Router,
  Route,
  Switch
} from 'react-router-dom';

import Home from './screens/home/Home';
import StravaAuth from './screens/stravaAuth/StravaAuth';
import StravaConfirmation from './screens/stravaAuth/StravaConfirmation';
import BfmPanel from './screens/bfmPanel/BfmPanel';
import AppTvboard from './screens/tvboard/AppTvboard';

import {authComponent, SignInHandler, SilentCallbackHandler} from "./components/Authentication/AuthComponent";
import SimpleLoginPage from "./components/Authentication/SimpleLoginPage";

function App(){
  let authBfmPanel = authComponent(BfmPanel, SimpleLoginPage);
  let authTvboard = authComponent(AppTvboard, SimpleLoginPage);

  return (
    <Router>
      <div className="App">
        <Switch>
          <Route exact path="/" component={Home} />
          <Route path="/strava" component={StravaAuth} />
          <Route path="/strava-confirmation" component={StravaConfirmation} />
          <Route path="/dashboard" component={authBfmPanel} />
          <Route path="/tvboard" component={authTvboard} />

          <Route path="/auth/signin" component={SignInHandler} />
          <Route path="/auth/silent" component={SilentCallbackHandler} />
        </Switch>
      </div>
    </Router>
  );
}

export default App;
