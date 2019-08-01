import React from 'react';
import './ActivitiesContribution.css';

const ActivitiesContribution = (props) =>{
  return (
    <div className="ActivitiesContribution">
      <p className="ActivitiesContribution-header">Activities Contribution</p>
      <p className="ActivitiesContribution-headerBest">Best result</p>
      <ul className="ActivitiesContribution-list">
        <li className="ActivitiesContribution-item">
          <div className="ActivitiesContribution-item__progress">
            <div className="ActivitiesContribution-item__category">
              Running
            </div>
            <div className="ActivitiesContribution-item__bar">
              <div className="ActivitiesContribution-item__barProgress">
              </div>
            </div>
          </div>
          <div className="ActivitiesContribution-item__best">
            <div className="ActivitiesContribution-item__bestName">
              Mariola Szklarz
            </div>
            <div className="ActivitiesContribution-item__bestResult">
              12 km
            </div>
          </div>
        </li>
      </ul>


    </div>
  )
}

export default ActivitiesContribution;
