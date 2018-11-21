import React, { Component } from 'react';
import {
  BrowserRouter as Router,
  Route
} from 'react-router-dom';

import Home from './screens/home/Home';
import StravaAuth from './screens/stravaAuth/StravaAuth';
import StravaConfirmation from './screens/stravaAuth/StravaConfirmation';

function App () {

  return (
    <Router>
      <div className="App">

        <Route exact path="/" component={Home} />
        <Route path="/strava" component={StravaAuth} />
        <Route path="/strava-confirmation" component={StravaConfirmation} />
      </div>
    </Router>
  );
}

export default App;
