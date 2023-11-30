import React, { Component } from 'react';
import axios from 'axios';
import Navbar from "./Navbar";
import '../styling/Appointments.css';
import '../styling/navbar.css';

export class Appointments extends Component {
    static displayName = Appointments.name;

    constructor(props) {
        super(props);
        this.state = {
            appointments: [],
        };
    }

    componentDidMount() {
        // Fetch appointments data

        try {
            axios.get("appointment", {
            })
                .then(response => {
                    const appointments = response.data; // Access the list of appointments
                    this.setState({ appointments }); // Update the state with fetched appointments
                });
        } catch (error) {
            console.log(error);
        }
    }

    render() {
        const { appointments } = this.state;

        return (
            <>
            <Navbar/>
            <div className="appointments-box">
                <h2>Henvisninger</h2>
                <div className="appointments-container">
                    <div className="appointment-header">
                        <div className="column">Sted</div>
                        <div className="column">Start tidspunkt</div>
                        <div className="column">Slut tidspunkt</div>
                    </div>
                    {appointments.map(appointment => (
                        <div key={appointment.Id} className="appointment-item">
                            <div className="column">{appointment.Place}</div>
                            <div className="column">{appointment.Start}</div>
                            <div className="column">{appointment.End}</div>
                        </div>
                    ))}
                </div>
                </div>
            </>
        );
    }
}
