import React, { Component } from 'react';
import { Route, Link, Switch } from 'react-router-dom';

import './screens/Dashboard/Dashboard.css';
import logo from 'static/img/logo-white.svg';
import iconRun from 'static/img/icon-run.svg';
import iconRide from 'static/img/icon-bike.svg';
import iconWalk from 'static/img/icon-walk.svg';
import iconWinterSports from 'static/img/icon-winter.svg';
import iconWaterSports from 'static/img/icon-water.svg';
import iconTeamSports from 'static/img/icon-team.svg';
import iconGym from 'static/img/icon-gym.svg';
import iconHike from 'static/img/icon-hike.svg';
import iconFitness from 'static/img/icon-fitness.svg';
import iconOther from 'static/img/icon-other.svg';

import Dashboard from './screens/Dashboard/Dashboard';
import Participants from './screens/Participants/Participants';
import NewActivity from './screens/NewActivity/NewActivity';
import AthletesList from './screens/AthletesList/AthletesList';
import AthleteProfile from './screens/AthleteProfile/AthleteProfile';

import StravaAuth from './screens/StravaAuth/StravaAuth';
import StravaConfirmation from './screens/StravaAuth/StravaConfirmation';

import IconRun from "static/img/IconRun";
import IconRide from "static/img/IconBike";
import IconWalk from "static/img/IconWalk";
import IconWinterSports from "static/img/IconWinter";
import IconWaterSports from "static/img/IconWater";
import IconTeamSports from "static/img/IconTeam";
import IconGym from "static/img/IconGym";
import IconHike from "static/img/IconHike";
import IconFitness from "static/img/IconFitness";
import IconOther from "static/img/IconOther";
import IconNewActivity from 'static/img/IconNewActivity';
import IconParticipants from 'static/img/IconParticipants';
import IconDashboard from 'static/img/IconDashboard';
import IconBeneficiaries from 'static/img/IconBeneficiaries';
import IconRules from 'static/img/IconRules';

import * as athletes_api from "../../api/endpoints/internal/athletes"
import * as categories_api from "../../api/endpoints/internal/categories";
import * as ranking_api from "../../api/endpoints/internal/ranking";

class AppDashboard extends Component {
  mobileViewport = window.matchMedia("screen and (max-width: 600px)");
  resizeTimeout;

  setCategoryDetails(category) {
    let icon, iconComponent, description;
    switch (category) {
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
      category: category,
      categoryIcon: icon,
      categoryIconComponent: iconComponent,
      categoryDescription: description
    };
    return obj;
  }
  setRankinkCategory = (category) => {
    if (category === this.state.rankingCategory) { return false; }
    this.setState({
      rankingCategory: category
    });
  }

  setRankingInputFilter = (input) => {
    this.setState({ rankingInputFilter: input })
  }

  handleResize = () => {
    clearTimeout(this.resizeTimeout);
    this.resizeTimeout = setTimeout(() => {
      this.setState({
        windowHeight: window.innerHeight,
        windowWidth: window.innerWidth
      });
      console.log('windowWidth', this.state.windowWidth);
    }, 300);
  }

  constructor(props) {
    super(props);
    this.state = {
      categories: [],
      athletes: [],
      ranking: [],
      rankingLoading: true,
      rankingCategory: 'All',
      rankingInputFilter: '',
      windowHeight: undefined,
      windowWidth: undefined
    }
  }

  render() {
    return (
      <div className="Dashboard">
        <aside className="Dashboard-side">
          <img className="Dashboard-logo" src={logo} alt="Burn for Money" />
          <ul className="Navigation">
            <li className="NavigationItem NavigationItem__dashboard">
              <Link to="/dashboard/" className="NavigationItem-link">
                <IconDashboard className="NavigationItem-icon" />
                <span className="NavigationItem-text">Dashboard</span>
              </Link>
            </li>
            <li className="NavigationItem NavigationItem__participants">
              <Link to="/dashboard/participants" className="NavigationItem-link">
                <IconParticipants className="NavigationItem-icon" />
                <span className="NavigationItem-text">Participants</span>
              </Link>
            </li>
            <li className="NavigationItem NavigationItem__beneficiaries">
              <Link to="/dashboard/athletes-list" className="NavigationItem-link">
                <IconBeneficiaries className="NavigationItem-icon" />
                <span className="NavigationItem-text">Beneficiaries</span>
              </Link>
            </li>
            <li className="NavigationItem NavigationItem__rules">
              <Link to="/dashboard/athletes-list" className="NavigationItem-link">
                <IconRules className="NavigationItem-icon" />
                <span className="NavigationItem-text">Rules</span>
              </Link>
            </li>
            <li className="NavigationItem NavigationItem__addActivity">
              <Link exact="true" to="/dashboard/new-activity" className="Button NavigationItem-link">
                <IconNewActivity className="NavigationItem-icon" {...(this.state.windowWidth <= 600 && { fill: '#EDA697' })} />
                <span className="NavigationItem-text">Add activity</span>
              </Link>
            </li>
          </ul>
        </aside>

        <section className="Dashboard-main">
          <Switch>
            <Route exact path="/dashboard" render={(props) => (
              <Dashboard {...props}
                ranking={this.state.ranking}
                rankingLoading={this.state.rankingLoading}
                rankingCategory={this.state.rankingCategory}
                setRankinkCategory={this.setRankinkCategory}
                categories={this.state.categories}
              />
            )} />
            <Route path="/dashboard/participants" render={(props) => (
              <Participants {...props}
                ranking={this.state.ranking}
                rankingLoading={this.state.rankingLoading}
                rankingCategory={this.state.rankingCategory}
                setRankinkCategory={this.setRankinkCategory}
                rankingInputFilter={this.state.rankingInputFilter}
                setRankingInputFilter={this.setRankingInputFilter}
                categories={this.state.categories}
              />
            )} />
            <Route exact path="/dashboard/new-activity" render={(props) => (
              <NewActivity {...props}
                categories={this.state.categories}
                athletes={this.state.athletes}
              />
            )} />
            <Route path="/dashboard/athletes-list" render={(props) => (
              <AthletesList {...props}
                athletes={this.state.athletes}
              />
            )} />
            <Route path="/dashboard/athlete/:athleteId" component={AthleteProfile} />
            <Route path="/dashboard/strava" component={StravaAuth} />
            <Route path="/dashboard/strava-confirmation" component={StravaConfirmation} />
          </Switch>
        </section>
      </div>
    )
  }

  componentDidMount() {
    this.handleResize();
    window.addEventListener("resize", this.handleResize);

    

    categories_api.getCategories()
      .then(
        (result) => {
        let categories = result.map((i) => { return this.setCategoryDetails(i) });
        this.setState({categories: categories});
      },
        (error) => {
          this.setState({categories: null});
          console.error('Error:', error);
        }
      );

    athletes_api.getAthletes()
      .then(
        (result) => {
          this.setState({athletes: result});
          console.log('athletes', this.state.athletes)
        },
        (error) => {
          this.setState({athletes: null});
          console.error('Error:', error);
        }
      );

    ranking_api.getRanking()
      .then(
        (result) => {
          this.setState({ ranking: result, rankingLoading: false });
          console.log('ranking', this.state.ranking)
        },
        (error) => {
          this.setState({ ranking: null });
          console.error('Error:', error);
        }
      );
  }

  componentWillUnmount() {
    window.removeEventListener("resize", this.handleResize);
  }

  componentDidUpdate(prevProps, prevState) {
    if (this.state.rankingCategory !== prevState.rankingCategory) {
      this.setState({ rankingLoading: true });
      let category = this.state.rankingCategory;
      if (category === 'All') category = '';

      ranking_api.getCategoryRanking(category)
        .then(
          (result) => { this.setState({ ranking: result, rankingLoading: false }); console.log('ranking', this.state.ranking) },
          (error) => { this.setState({ ranking: null }); console.error('Error:', error); }
        );
    }
  }
}

export default AppDashboard;
