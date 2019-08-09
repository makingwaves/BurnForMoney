import React from 'react';
import './ProgressBonus.css';

import finishingLine from 'static/img/finishing-line.svg';


const ProgressBonus = (props) =>{

  const countProgress = (percent) => {
    const minWidth = 60;
    return `calc(${minWidth}px + ${percent}% - (${percent} / 100 * ${minWidth}px))`;
  }

  return (
    <div className="ProgressBonus">
      <p className="ProgressBonus-motivationText">Keep going! We miss only <strong>55 pt</strong> for next 100 PLN</p>
      <div className="ProgressBonus-bar">
        <div className="ProgressBonus-barProgress" style={{width: countProgress(75)}}>
          <span className="ProgressBonus-barCurrentValue">0 pt</span>
        </div>
        <span className="ProgressBonus-barMaxValue">500 pt</span>

      </div>
      <div className="ProgressBonus-finishingLine">
        <img src={finishingLine} alt="" />
      </div>
    </div>
  )
}

export default ProgressBonus;
