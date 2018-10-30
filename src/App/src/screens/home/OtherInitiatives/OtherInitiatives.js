import React, { Component } from 'react';

import './OtherInitiatives.css';

class OtherInitiatives extends Component {
  render() {
    return (
      <div className="OtherInitiatives">
        <div className="OtherInitiatives__container container">
          <h2 className="OtherInitiatives__header Header">Other Initiatives</h2>
          <p className="OtherInitiatives__text">Conferences, employees support, events sponsorship<br/> and other charity programmes</p>
          <a href="http://praca.makingwaves.com/#initiatives" className="OtherInitiatives__button">See our initiatives</a>
        </div>
      </div>
    );
  }
}

export default OtherInitiatives;
