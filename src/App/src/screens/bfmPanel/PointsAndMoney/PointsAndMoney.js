import React from 'react';
import './PointsAndMoney.css';

import medal from 'static/img/medal.svg';
import money from 'static/img/money.svg';

const PointsAndMoney = (props) =>{
  return (
    <div className="PointsAndMoney">
    
      <div className="PointsAndMoney-container PointsAndMoney-Points">
        <div className="PointsAndMoney-score">
          <span className="PointsAndMoney-value">2045</span>
          <span className="PointsAndMoney-unit">pt</span>
        </div>
        <div className="PointsAndMoney-description">collected from activities</div>
        <img className="PointsAndMoney-icon" src={medal} alt="" />
      </div>

      <div className="PointsAndMoney-container PointsAndMoney-Money">
        <div className="PointsAndMoney-score">
          <span className="PointsAndMoney-value">400</span>
          <span className="PointsAndMoney-unit">pln</span>
        </div>
        <div className="PointsAndMoney-description">exchanges from points</div>
        <img className="PointsAndMoney-icon" src={money} alt="" />
      </div>

    </div>
  )
}

export default PointsAndMoney;
