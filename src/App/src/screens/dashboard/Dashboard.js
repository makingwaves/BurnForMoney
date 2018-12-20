import React, { Component } from 'react';
import { Route, Link, Switch } from 'react-router-dom';

import './Dashboard.css';
import logo from 'img/logo-white.svg';
import iconRun from 'img/icon-run.svg';
import iconRide from 'img/icon-bike.svg';
import iconWalk from 'img/icon-walk.svg';
import iconWinterSports from 'img/icon-winter.svg';
import iconWaterSports from 'img/icon-water.svg';
import iconTeamSports from 'img/icon-team.svg';
import iconGym from 'img/icon-gym.svg';
import iconHike from 'img/icon-hike.svg';
import iconFitness from 'img/icon-fitness.svg';
import iconOther from 'img/icon-other.svg';

import NewActivity from './NewActivity/NewActivity';
import AthletesList from './AthletesList/AthletesList';
import AthleteProfile from './AthleteProfile/AthleteProfile';

import IconRun from "img/IconRun";
import IconRide from "img/IconBike";
import IconWalk from "img/IconWalk";
import IconWinterSports from "img/IconWinter";
import IconWaterSports from "img/IconWater";
import IconTeamSports from "img/IconTeam";
import IconGym from "img/IconGym";
import IconHike from "img/IconHike";
import IconFitness from "img/IconFitness";
import IconOther from "img/IconOther";


class Dashboard extends Component {
  api_url = process.env.REACT_APP_DASHBOARD_API_URL;

  setCategoryDetails(category){
    let icon, iconComponent, description;
    switch(category){
      case 'Ride':
        icon = iconRide;
        iconComponent = IconRide;
        description = "in a park, on a gym, elliptical";
        break;
      case 'Run':
        icon = iconRun;
        iconComponent = IconRun;
        description = "in a park, on a gym, elliptical";
        break;
      case 'Walk':
        icon = iconWalk;
        iconComponent = IconWalk;
        description = "in a park, on a gym, elliptical";
        break;
      case 'WinterSports':
        icon = iconWinterSports;
        iconComponent = IconWinterSports;
        description = "in a park, on a gym, elliptical";
        break;
      case 'WaterSports':
        icon = iconWaterSports;
        iconComponent = IconWaterSports;
        description = "in a park, on a gym, elliptical";
        break;
      case 'TeamSports':
        icon = iconTeamSports;
        iconComponent = IconTeamSports;
        description = "in a park, on a gym, elliptical";
        break;
      case 'Gym':
        icon = iconGym;
        iconComponent = IconGym;
        description = "in a park, on a gym, elliptical";
        break;
      case 'Hike':
        icon = iconHike;
        iconComponent = IconHike;
        description = "in a park, on a gym, elliptical";
        break;
      case 'Fitness':
        icon = iconFitness;
        iconComponent = IconFitness;
        description = "in a park, on a gym, elliptical";
        break;
      case 'Other':
        icon = iconOther;
        iconComponent = IconOther;
        description = "in a park, on a gym, elliptical";
        break;
      default:
        icon = iconOther;
        iconComponent = IconOther;
        description = "in a park, on a gym, elliptical";
        break;
    }
    const obj = {
      category:  category,
      categoryIcon: icon,
      categoryIconComponent: iconComponent ,
      categoryDescription: description
    };
    return obj;
  }

  constructor(props) {
    super(props);
    this.state = {
      categories: []
    }
  }

  render() {
    return (
      <div className="Dashboard">
        <aside className="Dashboard-side">
          <img className="Dashboard-logo" src={logo} alt="Burn for Money" />
          <ul className="Dashboard-navigation">

            <li className="Dashboard-navigationItem"><Link to="/dashboard/athletes-list" className="Dashboard-navigationItem-link">Athletes List</Link></li>
          </ul>
          <Link exact="true" to="/dashboard/new-activity" className="Button Dashboard-navigation-addActivity">Add activity</Link>
        </aside>
        <section className="Dashboard-main">
          <Switch>
            <Route exact path="/dashboard/new-activity" render={(props) => (
              <NewActivity {...props} categories={this.state.categories}/>
            )} />
            <Route path="/dashboard/athletes-list" component={AthletesList} />
            <Route path="/dashboard/athlete/:athleteId" component={AthleteProfile} />
          </Switch>
        </section>
      </div>
    )
  }

  componentDidMount(){
    // internal api_url
    fetch(this.api_url+"api/activities/categories")
      .then(res => res.json())
      .then(
        (result) => { let categories = result.map( (i) => { return this.setCategoryDetails(i)}); this.setState({categories: categories }); console.log('STATE', this.state.categories)/**/},
        (error) => {this.setState({categories: null}); console.error('Error:', error); }
      );
  }
}

export default Dashboard;
