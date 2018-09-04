import React, { Component } from 'react';

import SliderItem from './SliderItem.js';
import './Slider.css';


class Slider extends Component {
  render() {
    return (
      <div className="Slider">
        <button href="#" className="Slider__prev"></button>
        <button href="#" className="Slider__next"></button>
        <div className="Slider__container">
          <ul className="Slider__list">
            <SliderItem/>
            <li>
              B
            </li>
            <li>
              C
            </li>
          </ul>
        </div>
      </div>
    );
  }
}

export default Slider;
