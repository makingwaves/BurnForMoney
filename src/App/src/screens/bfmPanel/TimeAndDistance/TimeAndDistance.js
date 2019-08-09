import React from 'react';
import './TimeAndDistance.css';

import clock from 'static/img/clock.svg';
import road from 'static/img/road.svg';

const TimeAndDistance = (props) =>{
  return (
    <div className="TimeAndDistance">

      <div className="TimeAndDistance-container TimeAndDistance-Points">
        <div className="TimeAndDistance-score">
          <span className="TimeAndDistance-value">25</span>
          <span className="TimeAndDistance-unit">h</span>
          <span className="TimeAndDistance-value">10</span>
          <span className="TimeAndDistance-unit">min</span>
        </div>
        <div className="TimeAndDistance-description">spent on activities</div>
        <img className="TimeAndDistance-icon" src={clock} alt="" />
      </div>

      <div className="TimeAndDistance-container TimeAndDistance-Money">
        <div className="TimeAndDistance-score">
          <span className="TimeAndDistance-value">400</span>
          <span className="TimeAndDistance-unit">km</span>
        </div>
        <div className="TimeAndDistance-description">on route</div>
        <img className="TimeAndDistance-icon" src={road} alt="" />
      </div>

    </div>
  )
}

export default TimeAndDistance;
