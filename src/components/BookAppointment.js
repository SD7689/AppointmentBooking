import React, { Component } from 'react';
import Navbar from 'react-bootstrap/Navbar'
import Container from 'react-bootstrap/Container'
import Row from 'react-bootstrap/Row'
import Col from 'react-bootstrap/Col'
import { Card } from 'react-bootstrap';
import logo from '../Bitmap (1).png'
import calendaricon from '../Group 10.png'
import personicon from '../Bitmap.png'
import { getAllBookingRequestMethod } from '../services/Services'
import Axios from 'axios';

export default class BookAppointment extends Component {
    constructor(props) {
        super(props)
        this.state = {
            BookingDetails: [],
            scheduleDate: [],
            SlotId: '',
            VisitedPeople: '',
            MaxCapacity: 20,
            Date1: '',
            Date2: '',
            Date3: '',
            AvailableSlot: '',
            ShowDates: false,
            ShowDates2: false,
            ShowDates3: false,
            counter: 0
        }
    }

    componentDidMount() {
        let StoreCode = "SMB3024";
        let tenantID = 1;
        Axios.post('https://localhost:44396/api/ChatbotBell/GetTimeSlotDetails', null, { params: { StoreCode, tenantID } })
            .then(response => {
                const BookingDetails = response.data;
                // const scheduleDate = response.data.dateofSchedules;
                // const scheduleDate = response.data;
                console.log(response.data)
                this.setState({
                    BookingDetails: BookingDetails,
                    //  scheduleDate: scheduleDate
                })
                console.log(this.state.BookingDetails)
                //   console.log(this.state.scheduleDate)

            })
    }

    // handleSubmit = event => {
    //     event.preventDefault()
    //     console.log(this.state)
    //             Axios.post('https://localhost:44396/api/ChatbotBell/GetBookingDetails', this.state)
    //                 .then(response => {
    //                     console.log(response)
    //                 })
    //                 .catch(error => {
    //                     console.log(error)
    //                 })
    // }

    showDateHandler = () => {
        this.setState(
            {
                ShowDates: true,
                ShowDates2: false,
                ShowDates3: false
            }
        )
    }

    showDateHandler2 = () => {
        this.setState(
            {
                ShowDates: false,
                ShowDates2: true,
                ShowDates3: false
            }
        )
    }

    showDateHandler3 = () => {
        this.setState(
            {
                ShowDates: false,
                ShowDates2: false,
                ShowDates3: true
            }
        )
    }

    incrementHandler = () => {
        let counter = this.state.counter;
        if (counter < 20) {
            this.setState(
                {
                    counter: counter + 1
                }
            )
        }
    }

    decrementHandler = () => {
        let counter = this.state.counter;
        if (counter !== 0) {
            this.setState(
                {
                    counter: counter - 1

                }
            )
        }
    }

    timeSlotHandler = () => {
        let SlotId = this.state.SlotId;
        this.setState(
            {
                SlotId: SlotId + 1
            }
        )
    }


    render() {
        // const { BookingDetails} = this.state;
        //  this.state.BookingDetails.map((ele) =>{
        return (
            <>
                <div className='booking-div'>


                    {/* {
                                        date.alreadyScheduleDetails.map((detail)=>{
                                            return(
                                                <>  */}




                    <div className='header-div'>
                        <Navbar bg="light">
                            <Navbar.Brand href="#home">

                                <img
                                    src={logo}
                                    width="96px"
                                    height="36px"
                                    className="d-inline-block"
                                    alt="logo"
                                />
                            </Navbar.Brand>
                        </Navbar>
                    </div>
                    {

                        this.state.BookingDetails.map((ele, i) => {
                            return (
                                <>
                                    <div className='address-div'>

                                        <h5 className='address-details'>{ele.storeName}</h5>
                                        <p className='address-bar'>{ele.storeAddress}</p>
                                        <h6 className='address-no'>{ele.storeContactDetails}</h6>

                                    </div>
                                    <div className="container-div">
                                        <Container fluid>
                                            <Row>
                                                <Col>
                                                    <div className="vist-div">
                                                        <h5 className="vist"><img
                                                            src={calendaricon}
                                                            width="16.2px"
                                                            height="18px"
                                                            className="d-inline-block"
                                                            alt="logo"
                                                        /> Schedule a Vist</h5>
                                                    </div>
                                                </Col>
                                            </Row>
                                            <Row>
                                                <hr className='hr-line'></hr>
                                                <div className="member-div">
                                                    <Col>
                                                        <div className='head-div'>
                                                            <h5 className='h5-font'>No. Of Members</h5>
                                                        </div>
                                                    </Col>
                                                    <Col></Col>
                                                    <Col>
                                                        <div className="button-div">
                                                            <img
                                                                src={personicon}
                                                                width="19px"
                                                                height="19px"
                                                                className="d-inline-block"
                                                                alt="logo"
                                                            />
                                                            <label placeholder="" className="input-div"><div className='label-div'>{this.state.counter}</div></label>
                                                            <button onClick={this.decrementHandler} className="btn-div"><i class="far fa-minus-circle"></i></button>
                                                            <button onClick={this.incrementHandler} className="btn-div-2"><i class="far fa-plus-circle"></i></button>
                                                        </div>
                                                    </Col>
                                                </div>
                                                <hr className='hr-line'></hr>
                                            </Row>

                                            <Col>
                                                <Row>
                                                    {ele.dateofSchedules.map((date) => {
                                                        return (
                                                            <>

                                                                <Card>


                                                                    <div className='date-div' key={date.id} onClick={this.showDateHandler} >
                                                                        <h5 className='h5-font'>{date.day}{date.dates}</h5>
                                                                    </div>
                                                                    {
                                                                        this.state.ShowDates ?
                                                                            // {date.alreadyScheduleDetails.map((date) => {
                                                                            //     return (
                                                                            <>

                                                                                <div className='time-details'>

                                                                                    <button className='time-btn' onClick={this.timeSlotHandler} >11AM-12PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler} >12PM-1PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>1AM-2PM</button>
                                                                                </div>
                                                                                <div className='row1'>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler} >2PM-3PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>3PM-4PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>4PM-5PM</button>
                                                                                </div>
                                                                                <div className='row2'>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler} >5PM-6PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>6PM-7PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>7PM-8PM</button>
                                                                                </div>
                                                                            </>
                                                                            : null}

                                                                </Card>
                                                                <hr className='hr-line'></hr>
                                                                <Card>
                                                                    <div className='date-div' onClick={this.showDateHandler2}>
                                                                        <h5 className='h5-font'></h5>
                                                                    </div>
                                                                    {
                                                                        this.state.ShowDates2 ?
                                                                            <>
                                                                                <div className='time-details'>

                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>11AM-12PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>12PM-1PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>1AM-2PM</button>
                                                                                </div>
                                                                                <div className='row1'>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>2PM-3PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>3PM-4PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>4PM-5PM</button>
                                                                                </div>
                                                                                <div className='row2'>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>5PM-6PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>6PM-7PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>7PM-8PM</button>
                                                                                </div>
                                                                            </>
                                                                            : null}
                                                                </Card>
                                                                <hr className='hr-line'></hr>
                                                                <Card>
                                                                    <div className='date-div' onClick={this.showDateHandler3}>
                                                                        <h5 className='h5-font'>Day After Tommrow 14th June</h5>
                                                                    </div>
                                                                    {
                                                                        this.state.ShowDates3 ?
                                                                            <>
                                                                                <div className='time-details'>

                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>11AM-12PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>12PM-1PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>1AM-2PM</button>
                                                                                </div>
                                                                                <div className='row1'>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>2PM-3PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>3PM-4PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>4PM-5PM</button>
                                                                                </div>
                                                                                <div className='row2'>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>5PM-6PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>6PM-7PM</button>
                                                                                    <button className='time-btn' onClick={this.timeSlotHandler}>7PM-8PM</button>
                                                                                </div>
                                                                            </>
                                                                            : null}
                                                                </Card>
                                                            </>
                                                        )
                                                    })
                                                    }
                                                </Row>
                                            </Col>
                                            <div className="Booking-btn">
                                                <button className='book-btn'>MAKE AN APPOINTMENT</button>
                                            </div>
                                        </Container>
                         
                    </div>
                    </>
                          )

                      })
                  }
                    {/* </>
                                            )
                                        })
                                    }
                    </>
                                )
                            }
                            )

                            } */}


                </div>
            </>
        );
        //    })
    }
}

