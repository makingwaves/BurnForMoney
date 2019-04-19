import React, { Component } from 'react';
import * as api from "../../api/endpoints/internal";

import './StravaAuth.css';
import logo from 'static/img/logo.svg';

import {AuthManager} from "../../components/Authentication/AuthManager"

class StravaAuth extends Component {

  getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

  constructor(props) {
      super(props);
      this.state = {
        strava_auth_finished: false
      };

      this._authManager = new AuthManager();
    }

  componentDidMount(){
    let code = this.getParameterByName("code");

    this._authManager.getUser()
    .then(user =>
      api.authorizeWithStrava(user.profile.sub, code))
    .then((r)=>{
      if(r.status === 200)
        this.setState({strava_auth_finished: true});
      else {
        console.log(r);
        alert("Strava account synchronization failed !");
      }
    })
    .catch((e)=>{
      console.log(e);
    });
  }

  render() {
    return (
      <div className="StravaAuth">
        <img src={logo} alt="Logo" className="logo"/>
        <p>Your account is synchronised!</p>
        <p>Now your points will be counted<br/>
        automatically for activity you log to Strava.</p>
        <div className="StravaAuth__confirmation">
          <div className="check_mark">
          {this.state.strava_auth_finished &&

            <div className="sa-icon sa-success animate">
              <span className="sa-line sa-tip animateSuccessTip"></span>
              <span className="sa-line sa-long animateSuccessLong"></span>
              <div className="sa-placeholder"></div>
              <div className="sa-fix"></div>
            </div>
          }

          </div>
        </div>
      </div>
    );
  }
}

export default StravaAuth;
