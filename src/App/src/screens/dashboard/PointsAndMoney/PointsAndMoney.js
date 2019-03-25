import React from 'react';
import './PointsAndMoney.css';

import money from 'img/money.svg';
import medal from 'img/medal.svg';


const PointsAndMoney = (props) =>{
  return (
    <div className="PointsAndMoney">
      <div className="PointsAndMoney-container PointsAndMoney-Points">
        <div className="PointsAndMoney-score">
          <span className="PointsAndMoney-value">2045</span>
          <span className="PointsAndMoney-unit">pt</span>
        </div>
        <p className="PointsAndMoney-description">collected from activities</p>
      </div>
      <div className="PointsAndMoney-container PointsAndMoney-Money">
        <div className="PointsAndMoney-score">
          <span className="PointsAndMoney-value">400</span>
          <span className="PointsAndMoney-unit">pln</span>
        </div>
        <p className="PointsAndMoney-description">exchanges from points</p>
      </div>
    </div>
  )
}

export default PointsAndMoney;
