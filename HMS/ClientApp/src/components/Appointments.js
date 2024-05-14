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
            editingAppointments: {},
            newAppointment: {
                place: '',
                start: '',
                end: ''
            },
            searchQuery: '',
            searchResult: null
        };
    }

    componentDidMount() {
        // Check if user is logged in.
        var token = localStorage.getItem("token");
        if (token === "") {
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

    handleSearchClick = async () => {
        const { searchQuery } = this.state;
        var request = "person/person/" + searchQuery;

        try {
            const response = await axios.get(request, {
                headers: { Authorization: localStorage.getItem('token') },
            });

            this.setState({ searchResult: response.data });

        } catch (error) {
            console.error('Error deleting journal:', error);
        }
    };

    handleAddAppointment = async () => {
        let { newAppointment } = this.state;

        // Additional information
        const userData = {
            userid: localStorage.getItem('userid')
        };

        // Merge additional info
        let updatedAppointment = {
            ...newAppointment,
            patient_id: userData.userid,
            doctor_id: 31,
            department_id: 1,
            hospital_id: 1
        };

        var request = "appointment/patientid/" + updatedAppointment.patient_id +
            "/doctorid/" + updatedAppointment.doctor_id +
            "/departmentid/" + updatedAppointment.department_id +
            "/hospitalid/" + updatedAppointment.hospital_id +
            "/start/" + updatedAppointment.start +
            "/end/" + updatedAppointment.end;

        try {
            await axios.get(request, {
                headers: { Authorization: localStorage.getItem('token') }
            });

            this.handleSendDataForSMTP();
            this.fetchAppointments();

        } catch (error) {
            console.log('Error adding new appointment: ', error);
        }
    };

    handleSendDataForSMTP = async () => {
        const { newAppointment } = this.state;
        const { searchQuery } = this.state;

        var request = "appointment/cpr/" + searchQuery +
            "/place/" + "København Sundhedshus" +
            "/start/" + newAppointment.start +
            "/end/" + newAppointment.end;

        try {
            const response = await axios.get(request, {
                headers: { Authorization: localStorage.getItem('token') }
            });

            if (response.data === true) {
                this.setState({ errorMessage: "Email sendt til patient!" });
            } else
            {
                this.setState({ errorMessage: "Kunne ikke oprette forbindelse til Email serveren." });
            }

        } catch (error) {
            this.setState({ errorMessage: "Kunne ikke oprette forbindelse til Email serveren." });
        }
    }

    getAppointmentById = (appointmentId) => {
        const { appointments } = this.state;
        return appointments.find((appointment) => appointment.Id === appointmentId);
    };

    render() {
        const { appointments, editingAppointments, newAppointment, searchQuery, searchResult } = this.state;

        // Define options for the dropdown
        const placeOptions = ["Kobenhavn Sundhedshus"];

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
                        <div className="appointment-item">
                            <div className="column">
                                <select
                                    value={newAppointment.place}
                                    onChange={(e) => this.setState({ newAppointment: { ...newAppointment, place: e.target.value } })}
                                >
                                    {placeOptions.map((option, index) => (
                                        <option key={index} value={option}>
                                            {option}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="column">
                                <input
                                    type="datetime-local"
                                    value={newAppointment.start}
                                    onChange={(e) => this.setState({ newAppointment: { ...newAppointment, start: e.target.value } })}
                                />
                            </div>
                            <div className="column">
                                <input
                                    type="datetime-local"
                                    value={newAppointment.end}
                                    onChange={(e) => this.setState({ newAppointment: { ...newAppointment, end: e.target.value } })}
                                />
                            </div>
                            <div className="column">
                                <input
                                    type="text"
                                    placeholder="Indtast cpr"
                                    value={searchQuery}
                                    onChange={(e) => this.setState({ searchQuery: e.target.value })}
                                />

                                {searchResult !== null && (
                                    <label className={searchResult ? 'success-label' : 'error-label'}>
                                        {searchResult === 0
                                            ? ''
                                            : searchResult === true
                                                ? 'Patient tilknyttet'
                                                : 'Patient findes ikke'}
                                    </label>
                                )}
                                <div style={{ marginTop: '10px' }}></div>
                                <button onClick={this.handleSearchClick}>Tilknyt patient</button>
                                <button onClick={this.handleAddAppointment}>Tilfoj</button>

                                {this.state.errorMessage && <p>{this.state.errorMessage}</p>}
                            </div>
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
                                        <span id="appointmentStart">{appointment.Start}</span>
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
                                        <span id="appointmentEnd">{appointment.End}</span>
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
