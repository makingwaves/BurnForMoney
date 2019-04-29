import React from 'react';
import './ProgressBonus.css';

import finishingLine from 'static/img/finishing-line.svg';


const ProgressBonus = (props) =>{
  return (
    <div className="ProgressBonus">
      <p className="ProgressBonus-motivationText">Keep going! We miss only <strong>55 pt</strong> for next 100 PLN</p>
      <div className="ProgressBonus-bar">
        <div className="ProgressBonus-barProgress">
        </div>
      </div>
      <div className="ProgressBonus-finishingLine">
        <img src={finishingLine} alt="" />
      </div>
    </div>
  )
}

export default ProgressBonus;
