import React from 'react';
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

import { withAdalLoginApi } from './adalConfig';

function App () {
  const ProtectedAppDashboard = withAdalLoginApi(AppDashboard, null, null);
  const ProtectedAppTvboard = withAdalLoginApi(AppTvboard, null, null);

  return (
    <Router>
      <div className="App">
        <Switch>
          <Route exact path="/" component={Home} />
          <Route path="/strava" component={StravaAuth} />
          <Route path="/strava-confirmation" component={StravaConfirmation} />
          <Route path="/dashboard" component={ProtectedAppDashboard} />
          <Route path="/tvboard" component={ProtectedAppTvboard} />
        </Switch>
      </div>
    </Router>
  );
}

export default App;
