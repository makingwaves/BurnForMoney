import React, { Component } from 'react';

import './NewActivity.css';
import iconDistance from 'img/icon-distance.svg';
import iconDuration from 'img/icon-duration.svg';

class NewActivity extends Component {
  api_url = process.env.REACT_APP_DASHBOARD_API_URL;
  categoriesWithDistance = ['Run', 'Ride', 'Walk'];

  constructor(props) {
    super(props);
    this.state = {
      showDistance: false,
      startDate: new Date().toISOString().substr(0, 10),
      category: '',
      distanceInKiloMeteres: '',
      movingTimeInMinutes: ''
    };
  }
  setCategory = (e) => {
    console.log('SETCategory:', e.target);
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
    const hhmm = this.state.movingTimeInMinutes;
    const t = hhmm.split(':');
    let timeInMinutes = (+t[0]) * 60 + (+t[1]);

    let newEntry = {
      "StartDate": this.state.startDate, // "yyyy-mm-dd",
      "Category": this.state.category,
      "DistanceInMeters": parseFloat(this.state.distanceInKiloMeteres, 10)*1000,
      "MovingTimeInMinutes": timeInMinutes
    };
    console.log("Adding new ENTRY: ",newEntry);
    fetch(this.api_url+"api/athlete/"+"3bf4898750c14f519bf7eee3027b3700"+"/activities", {
        method: 'POST',
        body: JSON.stringify(newEntry)
    }).then(
        (result) => { console.log('Result:', result)},
        (error) => { console.error('Error:', error) }
    );
  };

  render() {
    console.log('category: '+ this.state.category, 'show:'+ this.state.showDistance);

    return (
      <div className="NewActivity">
        <h3 className="NewActivity__header">New activity entry</h3>
        <div className="NewActivity__form">
          <div className="NewActivity__form-row">
            <label htmlFor="activityDate" className="NewActivity__form-label">Date</label>
            <input id="activityDate" type="date" required value={this.state.startDate} onChange={(e) => this.setState({startDate: e.target.value})}/>
          </div>

          <div className="NewActivity__tiles">
            {this.props.categories.map( (i) =>
              <div key={i.category} data-category={i.category} onClick={(e) => this.setCategory(e)} className={`NewActivity__tilesItem ${this.state.category === i.category ? 'active' : ''}`}>
                <i.categoryIconComponent className="NewActivity__tilesItem-iconComponent" />
                {/* <img src={i.categoryIcon} alt={i.category} className="NewActivity__tilesItem-icon" /> */}
                <h6 className="NewActivity__tilesItem-category">{i.category}</h6>
                <p className="NewActivity__tilesItem-description">{i.categoryDescription}</p>
              </div>)}
          </div>

          {this.state.showDistance && (
          <div className="NewActivity__form-row">
            <label htmlFor="activityDistance" className="NewActivity__form-label">
              <img src={iconDistance} alt="distance" className="NewActivity__form-labelImg" />Distance
            </label>
            <input id="activityDistance" type="number" className="NewActivity__form-input" placeholder="Distance in km" value={this.state.distanceInKiloMeteres} onChange={(e) => this.setState({distanceInKiloMeteres: e.target.value })} />
          </div>
          )}

          <div className="NewActivity__form-row">
            <label htmlFor="activityDuration" className="NewActivity__form-label">
              <img src={iconDuration} alt="duration" className="NewActivity__form-labelImg" />Duration
            </label>
            <input id="activityDuration" type="text" className="NewActivity__form-input" placeholder="Time in minutes" step="600" value={this.state.movingTimeInMinutes} onChange={(e) => this.setState({movingTimeInMinutes: e.target.value })} />
          </div>

          <div className="NewActivity__form-row">
            <input type="button" value="Save" className="Button NewActivity__form-save" onClick={this.addNewActivity}/>
          </div>
        </div>
      </div>
    );
  }

  componentDidMount(){
    // internal api_url
    fetch(this.api_url+"api/activities/categories")
      .then(res => res.json())
      .then(
        (result) => {this.setState({categories: result }); },
        (error) => {this.setState({categories: null}); console.error('Error:', error); }
      );
  }


}
export default NewActivity;
