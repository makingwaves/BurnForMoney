import React, { Component } from 'react';
import Modal from 'react-modal';

import './VideoHeader.css';
import logoMW from 'static/img/mw-logo-white.svg';
import logoBFMblue from '../../img/bfm-logo-blue.svg';
import awardCSR from '../../img/award-csr.svg';
import bfmVideoBg from 'static/video/bfm-www-movie.mp4';
import bfmVideo from 'static/video/bfm-www-movie-full.mp4';

import { withNamespaces } from 'react-i18next';

const customStyles = {
  content : {
    top                   : '50%',
    left                  : '52%',
    right                 : 'auto',
    bottom                : 'auto',
    marginRight           : '-50%',
    transform             : 'translate(-52%, -50%)'
  }
};
let videoInModal;

function updateVideoDimensions(video){
  let videoRatio = video.videoWidth / video.videoHeight,
      windowRatio = window.innerWidth / window.innerHeight;
  if(videoRatio < windowRatio){
    video.height = window.innerHeight - 100;
  } else {
    video.width = window.innerWidth - 100;
  }
}

// setAppElement() - hide application from screenreaders and other assistive technologies while the modal is open
Modal.setAppElement('#root');

class VideoHeader extends Component {
  constructor() {
    super();

    this.state = {
      modalIsOpen: false
    };
  }
  handleResize() {
    updateVideoDimensions(videoInModal);
  }
  openModal = () => {
    this.setState({modalIsOpen: true});
  }

  afterOpenModal = () => {
    videoInModal = document.querySelector('.VideoHeader__video');
    videoInModal.addEventListener( "loadedmetadata", function handleLoadmetadata() {
      updateVideoDimensions(videoInModal);
      videoInModal.removeEventListener( "loadedmetadata", handleLoadmetadata);
    });
    window.addEventListener("resize", this.handleResize)
  }

  closeModal = () => {
    this.setState({modalIsOpen: false});
    window.removeEventListener( "resize", this.handleResize);
  }
  goDown() {
    window.scroll({
      top: window.innerHeight,
      behavior: 'smooth'
    });
  }

  render() {
    const { t } = this.props;

    return (
      <div className="VideoHeader">
        <video loop muted autoPlay className="VideoHeader__background">
            <source src={bfmVideoBg} type="video/mp4"/>
        </video>
        <a href="http://makingwaves.com">
          <img src={logoMW} alt="Making Waves" className="VideoHeader__logoMw" />
        </a>
        <img src={awardCSR} alt="1. miejsce Dobra Praktyka CSR" className="VideoHeader__award" />

        <div className="VideoHeader__content">
          <img src={logoBFMblue} alt="Burn for money" className="VideoHeader__logo" />
          <div className="VideoHeader__title">
            <span className="VideoHeader__title-line">{t('intro line1')}</span><br/>
            <span className="VideoHeader__title-line">{t('intro line2')}</span><br/>
            <span className="VideoHeader__title-line">{t('intro line3')}</span>
          </div>
          <button className="VideoHeader__openVideo" onClick={this.openModal} >{t('watch in action')}</button>
        </div>
        <button className="VideoHeader__goDown" onClick={this.goDown}></button>

        <Modal
          isOpen={this.state.modalIsOpen}
          onAfterOpen={this.afterOpenModal}
          onRequestClose={this.closeModal}
          contentLabel="Video Modal"
          style={customStyles}
        >
          <video loop controls autoPlay className="VideoHeader__video">
              <source src={bfmVideo} type="video/mp4"/>
          </video>
          <button className="VideoHeader__close" onClick={this.closeModal}></button>
        </Modal>
      </div>
    );
  }

}

export default withNamespaces()(VideoHeader);
