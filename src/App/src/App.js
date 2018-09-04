import React, { Component } from 'react';
import {
  BrowserRouter as Router,
  Route,
  Link
} from 'react-router-dom';

import Home from './screens/home/Home';
import StravaAuth from './screens/stravaAuth/StravaAuth';


class App extends Component {
  render() {
    return (
      <Router>
        <div className="container">
          {/*
          <ul>
            <li>
              <Link to="/">Home</Link>
            </li>
            <li>
              <Link to="/strava">Strava Authorization</Link>
            </li>
          </ul>
          */}
          <Route exact path="/" component={Home} />
          <Route path="/strava" component={StravaAuth} />
        </div>
      </Router>
    );
  }
}

export default App;
