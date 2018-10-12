import React, { Component } from 'react';

import './AboutEngagement.css';

class AboutEngagement extends Component {
  render() {
    return (
      <div className="AboutEngagement">
        <div className="AboutEngagement__container container">
          <h2 className="AboutEngagement__header Header">About Engagement</h2>
          <p className="AboutEngagement__text">Conferences, employees support, events sponsorship<br/> and other charity programmes</p>
          <a href="http://makingwaves.com/" className="AboutEngagement__button">See our initiatives</a>
        </div>
      </div>
    );
  }
}

export default AboutEngagement;
