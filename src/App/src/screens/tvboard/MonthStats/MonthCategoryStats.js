import React, { Component } from 'react';

import './MonthStats.css';

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
import { withNamespaces } from 'react-i18next';

class MonthCategoryStats extends Component {

    setIcon(category){
        let icon;
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
          default:
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
        
            frequentActivitiesList = mostFrequentActivities.map( i =>
                <div className="MonthCategoryStats__category" key={i.category}>
                <div className="MonthCategoryStats__category__icon">
                    <img src={this.setIcon(i.category)} alt={i.category} />
                </div>
                <div className="MonthCategoryStats__category__progress">
                    <div className="MonthCategoryStats__category__progress-bar" style={{ width : (i.points / thisMonthPoints)*100 + '%' }}></div>
                    <div className="MonthCategoryStats__category__progress-value">{i.points} pt</div>
                </div>
                </div>
            );
        }

        return (
            <div >
                {frequentActivitiesList}
            </div>
        );
    }
}

export default withNamespaces()(MonthCategoryStats);