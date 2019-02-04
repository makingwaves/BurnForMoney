import React, { Component } from 'react';
import './NewActivity.css';

import DashboardHeader from '../DashboardHeader/DashboardHeader.js';

import iconDistance from 'img/icon-distance.svg';
import iconDuration from 'img/icon-duration.svg';

import {adalApiFetch} from "../../../adalConfig"

class NewActivity extends Component {
  api_url = process.env.REACT_APP_DASHBOARD_API_URL;
  categoriesWithDistance = ['Run', 'Ride', 'Walk'];

  constructor(props) {
    super(props);
    this.state = {
      showDistance: false,
      athleteId: localStorage.getItem('athleteId') || '',
      startDate: new Date().toISOString().substr(0, 10),
      category: '',
      distanceInKiloMeteres: '',
      movingTimeInHours: 0,
      movingTimeInMinutes: 0
    };
  }
  setCategory = (e) => {
    if(this.categoriesWithDistance.includes(e.target.getAttribute('data-category'))){
      this.setState({
        category: e.target.getAttribute('data-category'),
        showDistance: true
      });
    } else {
      this.setState({
        category: e.target.getAttribute('data-category'),
        showDistance: false
      });
    }
  }

  addNewActivity = () => {
    let timeInMinutes = (this.state.movingTimeInHours) * 60 + (this.state.movingTimeInMinutes)*1;
    let distanceInMeters = parseFloat(this.state.distanceInKiloMeteres, 10)*1000;

    let newEntry = {
      "StartDate": this.state.startDate, // "yyyy-mm-dd",
      "Type": this.state.category,
      "DistanceInMeters": distanceInMeters,
      "MovingTimeInMinutes": timeInMinutes
    };
    console.log("Adding new ENTRY: ",newEntry);
    console.log("AthleteId: ", this.state.athleteId);

    if(this.validate() ){
      adalApiFetch(this.api_url+"api/athlete/"+this.state.athleteId+"/activities", {
          method: 'POST',
          body: JSON.stringify(newEntry)
      }).then(
          (result) => { console.log('RESULT:', result)},
          (error) => { console.error('Error:', error) }
      );
    }
  };
  validate = () => {
    let timeInMinutes = (this.state.movingTimeInHours) * 60 + (this.state.movingTimeInMinutes)*1;
    let distanceInMeters = parseFloat(this.state.distanceInKiloMeteres, 10)*1000;
    let isValid = true;

    var elements = document.getElementsByClassName('error');
    while(elements.length > 0){
        elements[0].parentNode.removeChild(elements[0]);
    }
    if(this.state.athleteId === ''){
      this.showError('Choose who You are', 'activityAthlete');
      isValid = false;
    }
    if(this.state.startDate === ''){
      this.showError('Type correct date', 'activityDateDiv');
      isValid = false;
    }
    if(this.state.category === ''){
      this.showError('Choose one category', 'activityCategoryDiv');
      isValid = false;
    }
    if(isNaN(timeInMinutes) || timeInMinutes <= 0){
      this.showError('Type correct time', 'activityDurationDiv');
      isValid = false;
    }
    if(this.categoriesWithDistance.includes(this.state.category) && (isNaN(distanceInMeters) || distanceInMeters <= 0) ){
      this.showError('Type correct distance', 'activityDistanceDiv');
      isValid = false;
    }

    return isValid;
  }

  showError = (errorMsg, where) => {
    console.log('----errorMsg', errorMsg);
    var errorDiv = document.createElement('div');
    errorDiv.className = 'error';
    errorDiv.innerHTML = errorMsg;
    document.getElementById(where).appendChild(errorDiv);
  }

  render() {
    return (
      <React.Fragment>
        <DashboardHeader header="Add activity" />
        <div className="Dashboard-content NewActivity">
          <div className="NewActivity__form">
            <div className="NewActivity__form-row" id="activityAthlete">
              <label htmlFor="activityAthletesName" className="NewActivity__form-label">I am</label>
              <div className="NewActivity__form-selectAthlete">
                <select id="activityAthletesName" required value={this.state.athleteId} onChange={(e) => {this.setState({athleteId: e.target.value}); localStorage.setItem('athleteId', e.target.value); console.log('--->',localStorage.getItem('athleteId')) } }>
                  <option value=''>no one...</option>
                  {this.props.athletes.map( (i) =>
                    <option key={i.id} value={i.id}>{`${i.firstName} ${i.lastName}`}</option>
                  )}
                </select>
              </div>
            </div>

            <div className="NewActivity__form-row" id="activityDateDiv">
              <label htmlFor="activityDate" className="NewActivity__form-label">Date</label>
              <input id="activityDate" type="date" required value={this.state.startDate} onChange={(e) => this.setState({startDate: e.target.value})} className="NewActivity__form-inputDate" />
            </div>

            <div className="NewActivity__form-row" id="activityCategoryDiv">
              <div className="NewActivity__tiles" >
                {this.props.categories.map( (i) =>
                  <div key={i.category} data-category={i.category} onClick={(e) => this.setCategory(e)} className={`NewActivity__tilesItem ${this.state.category === i.category ? 'active' : ''}`}>
                    <i.categoryIconComponent className="NewActivity__tilesItem-iconComponent" />
                    <h6 className="NewActivity__tilesItem-category">{i.category}</h6>
                    <p className="NewActivity__tilesItem-description">{i.categoryDescription}</p>
                  </div>
                )}
              </div>
            </div>

            <div className="NewActivity__form-row" id="activityDurationDiv">
              <label htmlFor="activityDurationHours" className="NewActivity__form-label">
                <img src={iconDuration} alt="duration" className="NewActivity__form-labelImg" />Duration
              </label>
              <div className="NewActivity__form-input Duration">
                <input id="activityDurationHours" type="number" min="0" max="24" className="NewActivity__form-inputHours" placeholder="00" step="1" value={this.state.movingTimeInHours} onChange={(e) => this.setState({movingTimeInHours: e.target.value })} />
                <div className="NewActivity__form-inputUnit">hr</div>
              </div>
              <div className="NewActivity__form-input Duration">
                <input id="activityDurationMinutes" type="number" min="0" max="59" className="NewActivity__form-inputMinutes" placeholder="00" step="10" value={this.state.movingTimeInMinutes} onChange={(e) => this.setState({movingTimeInMinutes: e.target.value })} />
                <div className="NewActivity__form-inputUnit">min</div>
              </div>
            </div>

            {this.state.showDistance && (
            <div className="NewActivity__form-row" id="activityDistanceDiv">
              <label htmlFor="activityDistance" className="NewActivity__form-label">
                <img src={iconDistance} alt="distance" className="NewActivity__form-labelImg" />Distance
              </label>
              <div className="NewActivity__form-input Distance">
                <input id="activityDistance" type="number" min="0" max="300" className="NewActivity__form-inputKilometeres" placeholder="Distance in" value={this.state.distanceInKiloMeteres} onChange={(e) => this.setState({distanceInKiloMeteres: e.target.value })} />
                <div className="NewActivity__form-inputUnit">kilometers</div>
              </div>
            </div>
            )}

            <div className="NewActivity__form-row">
              <input type="button" value="Save" className="Button NewActivity__form-save" onClick={this.addNewActivity}/>
            </div>
          </div>
        </div>
      </React.Fragment>
    );
  }

  componentDidMount(){
    // internal api_url
    adalApiFetch(this.api_url+"api/activities/categories")
      .then(res => res.json())
      .then(
        (result) => {this.setState({categories: result }); },
        (error) => {this.setState({categories: null}); console.error('Error:', error); }
      );
  }


}
export default NewActivity;
