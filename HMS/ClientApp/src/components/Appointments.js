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
            editingAppointments: {}
        };
    }

    componentDidMount() {
        // Check if user is logged in.
        var token = localStorage.getItem("token");
        if (token === "")
        {
            window.location.href = '/';
            return;
        }

        // Fetch appointments data
        this.fetchAppointments();
    }

    async fetchAppointments() {
        try {
            const request = "appointment/patient/" + localStorage.getItem('userid');
            const response = await axios.get(request, {
                headers: { "Authorization": localStorage.getItem('token') }
            });
            const appointments = response.data;
            this.setState({ appointments });
        } catch (error) {
            console.log(error);
        }
    }

    handleUpdateClick = (appointmentId) => {
        const { editingAppointments } = this.state;

        // Set the appointment as being edited
        this.setState({
            editingAppointments: {
                ...editingAppointments,
                [appointmentId]: {
                    editing: true,
                    title: this.getAppointmentById(appointmentId).Title, // Save the original title
                    start: this.getAppointmentById(appointmentId).Start, // Save the original start date
                    end: this.getAppointmentById(appointmentId).End, // Save the original end date
                },
            },
        });
    };

    handleAppointmentChange = (appointmentId, field, e) => {
        const { editingAppointments } = this.state;

        this.setState({
            editingAppointments: {
                ...editingAppointments,
                [appointmentId]: {
                    ...editingAppointments[appointmentId],
                    [field]: e.target.value,
                },
            },
        });
    };

    handleSaveClick = async (appointmentId) => {
        const { editingAppointments } = this.state;

        try {
            var request = "appointment/" + appointmentId + "/start/" + editingAppointments[appointmentId]?.start + "/end/" + editingAppointments[appointmentId]?.end;

            axios.get(request, {
                headers: { "Authorization": localStorage.getItem('token') }
            });
        } catch (error) {
            console.log(error);
        }

        // Clear the editing state to reset the UI
        this.setState((prevState) => ({
            editingAppointments: {
                ...prevState.editingAppointments,
                [appointmentId]: { editing: false, title: '', start: '', end: '' },
            },
            appointments: [...prevState.appointments], // Update appointments in the state
        }));
        // Fetch appointments data
        await this.fetchAppointments();
    };

    handleDeleteClick = async (appointmentId) => {
        try {
            var request = "appointment/" + appointmentId;
            axios.delete(request, {
                headers: { "Authorization": localStorage.getItem('token') }
            });

            this.setState(prevState => ({
                appointments: prevState.appointments.filter(appointment => appointment.Id !== appointmentId)
            }));

        } catch (error) {
            console.log(error);
        }
    };

    getAppointmentById = (appointmentId) => {
        const { appointments } = this.state;
        return appointments.find((appointment) => appointment.Id === appointmentId);
    };

    render() {
        const { appointments, editingAppointments } = this.state;

        return (
            <>
                <Navbar />
                <div className="appointments-box">
                    <h2>Henvisninger</h2>
                    <div className="appointments-container">
                        <div className="appointment-header">
                            <div className="column">Sted</div>
                            <div className="column">Start tidspunkt</div>
                            <div className="column">Slut tidspunkt</div>
                            <div className="column">Handlinger</div>
                        </div>
                        {appointments.map(appointment => (
                            <div key={appointment.Id} className="appointment-item">
                                <div className="column">
                                    <span>{appointment.Place}</span>
                                </div>
                                <div className="column">
                                    {editingAppointments[appointment.Id]?.editing ? (
                                        <input
                                            type="datetime-local"
                                            value={editingAppointments[appointment.Id]?.start}
                                            onChange={(e) => this.handleAppointmentChange(appointment.Id, 'start', e)}
                                        />
                                    ) : (
                                        <span>{appointment.Start}</span>
                                    )}
                                </div>
                                <div className="column">
                                    {editingAppointments[appointment.Id]?.editing ? (
                                        <input
                                            type="datetime-local"
                                            value={editingAppointments[appointment.Id]?.end}
                                            onChange={(e) => this.handleAppointmentChange(appointment.Id, 'end', e)}
                                        />
                                    ) : (
                                        <span>{appointment.End}</span>
                                    )}
                                </div>
                                <div className="column">
                                    {editingAppointments[appointment.Id]?.editing ? (
                                        <button onClick={() => this.handleSaveClick(appointment.Id)}>Gem</button>
                                    ) : (
                                        <>
                                            <button onClick={() => this.handleUpdateClick(appointment.Id)}>
                                                Opdater
                                            </button>
                                            <span className="button-spacing" />
                                            <button onClick={() => this.handleDeleteClick(appointment.Id)}>
                                                Slet
                                            </button>
                                        </>
                                    )}
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
            </>
        );
    }
}
