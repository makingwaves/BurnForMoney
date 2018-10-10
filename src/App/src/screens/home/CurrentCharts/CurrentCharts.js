import React, { Component } from 'react';

import './CurrentCharts.css';

import iconBike from 'img/icon-bike.svg';
import iconRun from 'img/icon-run.svg';
import iconFootball from 'img/icon-football.svg';

class CurrentCharts extends Component {
  render() {
    return (
      <div className="CurrentCharts">
        <div className="CurrentCharts__container container">

          <h2 className="CurrentCharts__header Header"><strong className="flames">We do sports</strong></h2>
          <h4>Current month</h4>
          <div className="CurrentCharts__stats">

            <div className="CurrentCharts__stats-overall">
              <div className="CurrentCharts__stats-trainings CurrentCharts__stats-block">
                <div className="CurrentCharts__stats-trainings__value CurrentCharts__stats-block__value">2246</div>
                <div className="CurrentCharts__stats-trainings__text">Trainings</div>
              </div>
              <div className="CurrentCharts__stats-engagement CurrentCharts__stats-block">
                <div className="CurrentCharts__stats-engagement__value CurrentCharts__stats-block__value">37%</div>
                <div className="CurrentCharts__stats-engagement__text">Making<br/>Wavers<br/>engaged</div>
              </div>
              <div className="CurrentCharts__stats-money CurrentCharts__stats-block">
                <div className="CurrentCharts__stats-money__value CurrentCharts__stats-block__value">2400 zł</div>
                <div className="CurrentCharts__stats-money__text">This month. So far.</div>
              </div>
            </div>

            <div className="CurrentCharts__stats-specific">

                <div className="CurrentCharts__stats-category__icon">
                  <img src={iconBike} alt="bike" />
                </div>
                <div className="CurrentCharts__stats-category__progress">
                  <div className="CurrentCharts__stats-category__progress-bar" style={{width:'92%'}}></div>
                  <div className="CurrentCharts__stats-category__progress-value">460 pt</div>
                </div>

                <div className="CurrentCharts__stats-category__icon">
                  <img src={iconRun} alt="bike" />
                </div>
                <div className="CurrentCharts__stats-category__progress">
                  <div className="CurrentCharts__stats-category__progress-bar" style={{width:'46%'}}></div>
                  <div className="CurrentCharts__stats-category__progress-value">230 pt</div>
                </div>

                <div className="CurrentCharts__stats-category__icon">
                  <img src={iconFootball} alt="bike" />
                </div>
                <div className="CurrentCharts__stats-category__progress">
                  <div className="CurrentCharts__stats-category__progress-bar" style={{width:'2%'}}></div>
                  <div className="CurrentCharts__stats-category__progress-value">10 pt</div>
                </div>

                <div className="CurrentCharts__stats-category__icon">
                  <img src={iconBike} alt="bike" />
                </div>
                <div className="CurrentCharts__stats-category__progress">
                  <div className="CurrentCharts__stats-category__progress-bar" style={{width:'60%'}}></div>
                  <div className="CurrentCharts__stats-category__progress-value">300 pt</div>
                </div>

                <div className="CurrentCharts__stats-category__icon">
                  <img src={iconRun} alt="bike" />
                </div>
                <div className="CurrentCharts__stats-category__progress">
                  <div className="CurrentCharts__stats-category__progress-bar" style={{width:'25%'}}></div>
                  <div className="CurrentCharts__stats-category__progress-value">125 pt</div>
                </div>

            </div>

          </div>

        </div>
      </div>
    );
  }
}

export default CurrentCharts;
