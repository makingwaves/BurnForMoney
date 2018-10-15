import React, { Component } from 'react';

import './CurrentCharts.css';

import iconRide from 'img/icon-bike.svg';
import iconRun from 'img/icon-run.svg';
import iconWalk from 'img/icon-walk.svg';
import iconWinterSports from 'img/icon-winter.svg';
import iconWaterSports from 'img/icon-water.svg';
import iconTeamSports from 'img/icon-team.svg';
import iconGym from 'img/icon-gym.svg';
import iconHike from 'img/icon-hike.svg';
import iconFitness from 'img/icon-fitness.svg';
import iconOther from 'img/icon-other.svg';

class CurrentCharts extends Component {

  setIcon(category){
    let icon = iconOther;
    switch(category){
      case 'Ride':
        icon = iconRide;
        break;
      case 'Run':
        icon = iconRun;
        break;
      case 'Walk':
        icon = iconWalk;
        break;
      case 'WinterSports':
        icon = iconWinterSports;
        break;
      case 'WaterSports':
        icon = iconWaterSports;
        break;
      case 'TeamSports':
        icon = iconTeamSports;
        break;
      case 'Gym':
        icon = iconGym;
        break;
      case 'Hike':
        icon = iconHike;
        break;
      case 'Fitness':
        icon = iconFitness;
        break;
      case 'Other':
        icon = iconOther;
        break;

    }
    return icon;
  }

  render() {

    let frequentActivitiesList;
    if(this.props.data){
      let mostFrequentActivities = this.props.data.thisMonth.mostFrequentActivities;
      let thisMonthPoints = this.props.data.thisMonth.points;
      console.log(iconRide);
      console.log('1', mostFrequentActivities);


      frequentActivitiesList = mostFrequentActivities.map( i =>

        <div className="CurrentCharts__stats-category" key={i.category}>
          <div className="CurrentCharts__stats-category__icon">
            <img src={this.setIcon(i.category)} alt={i.category} />
          </div>
          <div className="CurrentCharts__stats-category__progress">
            <div className="CurrentCharts__stats-category__progress-bar" style={{ width : (i.points / thisMonthPoints)*100 + '%' }}></div>
            <div className="CurrentCharts__stats-category__progress-value">{i.points} pt</div>
          </div>
        </div>
      )
    }


    return (
      <div className="CurrentCharts">
        <div className="CurrentCharts__container container">

          <h2 className="CurrentCharts__header Header"><strong className="flames">We do sports</strong></h2>
          <h4>Current month</h4>
          <div className="CurrentCharts__stats">

            <div className="CurrentCharts__stats-overall">
              <div className="CurrentCharts__stats-trainings CurrentCharts__stats-block">
                <div className="CurrentCharts__stats-trainings__value CurrentCharts__stats-block__value">{(this.props.data ? this.props.data.thisMonth.numberOfTrainings : 0)}</div>
                <div className="CurrentCharts__stats-trainings__text">Trainings</div>
              </div>
              <div className="CurrentCharts__stats-engagement CurrentCharts__stats-block">
                <div className="CurrentCharts__stats-engagement__value CurrentCharts__stats-block__value">{(this.props.data ? this.props.data.thisMonth.percentOfEngagedEmployees : 0)}%</div>
                <div className="CurrentCharts__stats-engagement__text">Making<br/>Wavers<br/>engaged</div>
              </div>
              <div className="CurrentCharts__stats-money CurrentCharts__stats-block">
                <div className="CurrentCharts__stats-money__value CurrentCharts__stats-block__value">{(this.props.data ? this.props.data.thisMonth.money : 0)} zł</div>
                <div className="CurrentCharts__stats-money__text">This month. So far.</div>
              </div>
            </div>

            <div className="CurrentCharts__stats-specific">
              {frequentActivitiesList}
                {/*
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
                */}
            </div>

          </div>

        </div>
      </div>
    );
  }
}

export default CurrentCharts;
